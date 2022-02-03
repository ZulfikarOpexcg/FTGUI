﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Camstar.WCF.ObjectStack;
using ComponentFactory.Krypton.Toolkit;
using OpcenterWikLibrary;

namespace FTGUI
{
    public partial class Main : KryptonForm
    {
        #region CONSTRUCTOR
        public Main()
        {
            InitializeComponent();
            Rectangle r = new Rectangle(0, 0, Pb_IndicatorPicture.Width, Pb_IndicatorPicture.Height);
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            int d = 28;
            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
            Pb_IndicatorPicture.Region = new Region(gp);
            this.WindowState = FormWindowState.Normal;
            this.Size = new Size(1400, 770);

            GetResourceStatusCodeList();
            GetStatusOfResource();
            GetStatusMaintenanceDetails();
            GetCarrierList();
            Cb_Carrier.SelectedItem = null;
            Cb_StatusCode.SelectedItem = null;
            Cb_StatusReason.SelectedItem = null;
            Tb_SetupAvailability.Text = "";

            MyTitle.Text = $"FT - {AppSettings.Resource}";
            ResourceGrouping.Values.Heading = $"Resource Status: {AppSettings.Resource}";
            ResourceSetupGrouping.Values.Heading = $"Resource Setup: {AppSettings.Resource}";
            ResourceDataGroup.Values.Heading = $"Resource Data Collection: {AppSettings.Resource}";
            AddVersionNumber();
        }
        #endregion

        #region INSTANCE VARIABLE
        private static GetMaintenanceStatusDetails[] oMaintenanceStatus = null;
        private static ServiceUtil oServiceUtil = new ServiceUtil();
        private int iTestNumber = 1;
        #endregion

        #region FUNCTION USEFULL
        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            this.Text += $" V.{versionInfo.FileVersion}";
        }
        #endregion

        #region FUNCTION STATUS OF RESOURCE
        private void GetResourceStatusCodeList()
        {
            NamedObjectRef[] oStatusCodeList = oServiceUtil.GetListResourceStatusCode();
            if (oStatusCodeList != null)
            {
                Cb_StatusCode.DataSource = oStatusCodeList;
            }
        }
        private void GetStatusMaintenanceDetails()
        {
            try
            {
                oMaintenanceStatus = oServiceUtil.GetGetMaintenanceStatus(AppSettings.Resource);
                if (oMaintenanceStatus != null)
                {
                    Dg_Maintenance.DataSource = oMaintenanceStatus;
                    Dg_Maintenance.Columns["Due"].Visible = false;
                    Dg_Maintenance.Columns["Warning"].Visible = false;
                    Dg_Maintenance.Columns["PastDue"].Visible = false;
                    Dg_Maintenance.Columns["MaintenanceReqName"].Visible = false;
                    Dg_Maintenance.Columns["MaintenanceReqDisplayName"].Visible = false;
                    Dg_Maintenance.Columns["ResourceStatusCodeName"].Visible = false;
                    Dg_Maintenance.Columns["UOMName"].Visible = false;
                    Dg_Maintenance.Columns["ResourceName"].Visible = false;
                    Dg_Maintenance.Columns["UOM2Name"].Visible = false;
                    Dg_Maintenance.Columns["MaintenanceReqRev"].Visible = false;
                    Dg_Maintenance.Columns["NextThruputQty2Warning"].Visible = false;
                    Dg_Maintenance.Columns["NextThruputQty2Limit"].Visible = false;
                    Dg_Maintenance.Columns["UOM2"].Visible = false;
                    Dg_Maintenance.Columns["ThruputQty2"].Visible = false;
                    Dg_Maintenance.Columns["Resource"].Visible = false;
                    Dg_Maintenance.Columns["ResourceStatusCode"].Visible = false;
                    Dg_Maintenance.Columns["NextThruputQty2Due"].Visible = false;
                    Dg_Maintenance.Columns["MaintenanceClassName"].Visible = false;
                    Dg_Maintenance.Columns["MaintenanceStatus"].Visible = false;
                    Dg_Maintenance.Columns["ExportImportKey"].Visible = false;
                    Dg_Maintenance.Columns["DisplayName"].Visible = false;
                    Dg_Maintenance.Columns["Self"].Visible = false;
                    Dg_Maintenance.Columns["IsEmpty"].Visible = false;
                    Dg_Maintenance.Columns["FieldAction"].Visible = false;
                    Dg_Maintenance.Columns["IgnoreTypeDifference"].Visible = false;
                    Dg_Maintenance.Columns["ListItemAction"].Visible = false;
                    Dg_Maintenance.Columns["ListItemIndex"].Visible = false;
                    Dg_Maintenance.Columns["CDOTypeName"].Visible = false;
                    Dg_Maintenance.Columns["key"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
            }
        }
        private void GetStatusOfResource()
        {
            try
            {
                ResourceStatusDetails oResourceStatusDetails = oServiceUtil.GetResourceStatusDetails(AppSettings.Resource);
                if (oResourceStatusDetails != null)
                {
                    if (oResourceStatusDetails.Status != null) Tb_StatusCode.Text = oResourceStatusDetails.Status.Name;
                    if (oResourceStatusDetails.Reason != null) Tb_StatusReason.Text = oResourceStatusDetails.Reason.Name;
                    if (oResourceStatusDetails.Availability != null)
                    {
                        Tb_Availability.Text = oResourceStatusDetails.Availability.Value;
                        if (oResourceStatusDetails.Availability.Value == "Up")
                        {
                            Pb_IndicatorPicture.BackColor = Color.Green;
                        }
                        else if (oResourceStatusDetails.Availability.Value == "Down")
                        {
                            Pb_IndicatorPicture.BackColor = Color.Red;
                        }
                    }
                    else
                    {
                        Pb_IndicatorPicture.BackColor = Color.Orange;
                    }
                    if (oResourceStatusDetails.TimeAtStatus != null) Tb_TimeAtStatus.Text = Convert.ToString(oResourceStatusDetails.TimeAtStatus.Value);
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
            }
        }
        private void GetCarrierList()
        {
            NamedObjectRef[] oCarrierList = oServiceUtil.GetCarrierList();
            if (oCarrierList != null)
            {
                Cb_Carrier.DataSource = oCarrierList;
            }
        }
        #endregion

        #region COMPONENT EVENT
        private void Bt_StartMove_Click(object sender, EventArgs e)
        {
            try
            {
                bool resultMoveIn = false;
                bool resultMoveStd = false;
                string sPassFail = Cb_PassFail.Text != "" ? Cb_PassFail.Text : "Fail";
                Camstar.WCF.ObjectStack.DataPointDetails[] cDataPoint = new Camstar.WCF.ObjectStack.DataPointDetails[16];
                cDataPoint[0] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Power Supply Voltage", DataValue = Tb_PSV.Text != ""? Tb_PSV.Text : "0" , DataType = DataTypeEnum.String };
                cDataPoint[1] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Frequency", DataValue = Tb_Freq.Text != "" ? Tb_Freq.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[2] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Stand by Power", DataValue = Tb_SBY.Text != "" ? Tb_SBY.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[3] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Flow Rate of Cold Water", DataValue = Tb_FRoCW.Text != "" ? Tb_FRoCW.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[4] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Temperature of Cold Water Out", DataValue = Tb_ToCWO.Text != "" ? Tb_ToCWO.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[5] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Temperature Delta", DataValue = Tb_TD.Text != "" ? Tb_TD.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[6] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Pressure Working Cold Water", DataValue = Tb_PWCW.Text != "" ? Tb_PWCW.Text : "0" , DataType = DataTypeEnum.String };
                cDataPoint[7] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Cold Water Energy Absorption", DataValue = Tb_CWEA.Text != "" ? Tb_CWEA.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[8] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Pressure Max", DataValue = Tb_PM.Text != "" ? Tb_PM.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[9] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Pressure Loss", DataValue = Tb_PL.Text != "" ? Tb_PL.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[10] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Flow Rate of Hot Water", DataValue = Tb_FRoHW.Text != "" ? Tb_FRoHW.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[11] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Pressure Working Hot Water", DataValue = Tb_PWHW.Text != "" ? Tb_PWHW.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[12] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Temperature of Hot Water Max", DataValue = Tb_ToHWM.Text != "" ? Tb_ToHWM.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[13] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Host Water Power Consumption", DataValue = Tb_HWPC.Text != "" ? Tb_HWPC.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[14] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Hot Water Energy Absorption", DataValue = Tb_HWEA.Text != "" ? Tb_HWEA.Text : "0", DataType = DataTypeEnum.String };
                cDataPoint[15] = new Camstar.WCF.ObjectStack.DataPointDetails() { DataName = "Pass/Fail", DataValue = sPassFail, DataType = DataTypeEnum.String };
                CurrentContainerStatus oContainerStatus = oServiceUtil.GetContainerStatusDetails(Tb_SerialNumber.Text, "FCT Data");
                if (oContainerStatus.ContainerName != null)
                {
                    resultMoveIn = oServiceUtil.ExecuteMoveIn(oContainerStatus.ContainerName.Value, this.iTestNumber == 1 ? AppSettings.Resource : "", "", "", null);
                    if (resultMoveIn)
                    {
                        resultMoveStd = oServiceUtil.ExecuteMoveStd(oContainerStatus.ContainerName.Value, "", this.iTestNumber == 1 ? AppSettings.Resource : "", "FCT Data", "", cDataPoint, Cb_Carrier.SelectedValue != null && this.iTestNumber == 1 ? Cb_Carrier.SelectedValue.ToString() : "", false);
                        if (resultMoveStd)
                        {
                            Tb_ContainerPosition.Text = oServiceUtil.GetCurrentContainerStep(Tb_SerialNumber.Text);
                            oServiceUtil.ExecuteResourceThruput(this.iTestNumber == 1 ? Cb_Carrier.Text : "", 1, "Unit", oContainerStatus.Product.Name.ToString());
                            oContainerStatus = oServiceUtil.GetContainerStatusDetails(Tb_SerialNumber.Text, "FCT Data");
                            if (sPassFail == "Fail" && this.iTestNumber == 1)
                            {
                                this.iTestNumber = 2;
                                Bt_Move.Text = "Move In and Move (2)";
                                MessageBox.Show($"The result Test-1 is Fail and will be perform Test-2.\nMoveIn and MoveStd success! Move to the Operation: {oContainerStatus.OperationName.Value}.");
                            }
                            else if (sPassFail == "Pass" && this.iTestNumber == 1)
                            {
                                this.iTestNumber = 1;
                                Bt_Move.Text = "Move In and Move (1)";
                                MessageBox.Show($"Test-1 the result is Pass.\nMoveIn and MoveStd success! Move to the Operation: {oContainerStatus.OperationName.Value}.");
                            }
                            else if (sPassFail == "Pass" && this.iTestNumber == 2)
                            {
                                this.iTestNumber = 1;
                                Bt_Move.Text = "Move In and Move (1)";
                                MessageBox.Show($"Test-2 the result is Pass.\nMoveIn and MoveStd success! Move to the Operation: {oContainerStatus.OperationName.Value}.");
                            }
                            else if (sPassFail == "Fail" && this.iTestNumber == 2)
                            {
                                this.iTestNumber = 1;
                                Bt_Move.Text = "Move In and Move (1)";
                                MessageBox.Show($"Test-2 the result is Fail.\nMoveIn and MoveStd success! Move to the Operation: {oContainerStatus.OperationName.Value}.");
                            }
                        }
                        else MessageBox.Show("Move In success and but Move Std Fail!");
                    }
                    else MessageBox.Show("Move In and Move Std Fail!");
                }
                else MessageBox.Show($"Container {Tb_SerialNumber.Text} not found!");
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
            }
        }
        private void Cb_StatusCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ResourceStatusCodeChanges oStatusCode = oServiceUtil.GetResourceStatusCode(Cb_StatusCode.SelectedValue != null ? Cb_StatusCode.SelectedValue.ToString() : "");
                if (oStatusCode != null)
                {
                    Tb_SetupAvailability.Text = oStatusCode.Availability.ToString();
                    if (oStatusCode.ResourceStatusReasons != null)
                    {
                        ResStatusReasonGroupChanges oStatusReason = oServiceUtil.GetResourceStatusReasonGroup(oStatusCode.ResourceStatusReasons.Name);
                        Cb_StatusReason.DataSource = oStatusReason.Entries;
                    }
                    else
                    {
                        Cb_StatusReason.Items.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
            }
        }
        private void Bt_SetResourceStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (Cb_StatusCode.Text != "" && Cb_StatusReason.Text != "")
                {
                    oServiceUtil.ExecuteResourceSetup(AppSettings.Resource, Cb_StatusCode.Text, Cb_StatusReason.Text);
                }
                else if (Cb_StatusCode.Text != "")
                {
                    oServiceUtil.ExecuteResourceSetup(AppSettings.Resource, Cb_StatusCode.Text, "");
                }
                GetStatusOfResource();
                GetStatusMaintenanceDetails();
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
            }
        }
        private void Bt_FindContainer_Click(object sender, EventArgs e)
        {
            Tb_Operation.Clear();
            Tb_PO.Clear();
            CurrentContainerStatus oContainerStatus = oServiceUtil.GetContainerStatusDetails(Tb_SerialNumber.Text, "HI-POT Data");
            Tb_ContainerPosition.Text = oServiceUtil.GetCurrentContainerStep(Tb_SerialNumber.Text);
            if (oContainerStatus != null)
            {
                if (oContainerStatus.MfgOrderName != null) Tb_PO.Text = oContainerStatus.MfgOrderName.ToString();
                if (oContainerStatus.Operation != null) Tb_Operation.Text = oContainerStatus.Operation.Name.ToString();
                if (oContainerStatus.Carrier != null) Cb_Carrier.Text = oContainerStatus.Carrier.Name.ToString();
            }
        }
        private void Dg_Maintenance_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in Dg_Maintenance.Rows)
                {
                    //Console.WriteLine(Convert.ToString(row.Cells["MaintenanceState"].Value));
                    if (Convert.ToString(row.Cells["MaintenanceState"].Value) == "Pending")
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                    else if (Convert.ToString(row.Cells["MaintenanceState"].Value) == "Due")
                    {
                        row.DefaultCellStyle.BackColor = Color.Orange;
                    }
                    else if (Convert.ToString(row.Cells["MaintenanceState"].Value) == "Past Due")
                    {
                        row.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
            }
        }
        private void TimerRealtime_Tick(object sender, EventArgs e)
        {
            GetStatusOfResource();
            GetStatusMaintenanceDetails();
        }
        private void Cb_StatusCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void Cb_StatusReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void Cb_PassFail_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

    }
}