using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WizMes_SeongBinFood.PopUp;
using WizMes_SeongBinFood.PopUP;
using WPF.MDI;

namespace WizMes_SeongBinFood
{
    /// <summary>
    /// Win_prd_HygieneCheck.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_prd_HygieneCheck : UserControl
    {
        string strFlag = string.Empty;
        int rowNum = 0;
       
        Win_prd_HygieneCheck_CodeView WinMcRegulInspect = new Win_prd_HygieneCheck_CodeView();
        Win_prd_HygieneCheck_Sub_CodeView WinMcRegulInspectSub = new Win_prd_HygieneCheck_Sub_CodeView();
        Lib lib = new Lib();

   
        private List<string> lstMsg = new List<string>();
        private string message = "";

        public Win_prd_HygieneCheck()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            SetComboBox();

            chkInspectDateSrh.IsChecked = true;
            btnThisMonth_Click(null, null);
        }

        private void SetComboBox()
        {
        }

        #region 체크박스 in 라벨 & PlusFinder

        //검사일자 라벨 클릭시
        private void lblInspectDateSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkInspectDateSrh.IsChecked == true) { chkInspectDateSrh.IsChecked = false; }
            else { chkInspectDateSrh.IsChecked = true; }
        }

        //검사일자 라벨 in 체크박스 체크시
        private void chkInspectDateSrh_Checked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = true;
            dtpEDate.IsEnabled = true;
            btnThisMonth.IsEnabled = true;
            btnToday.IsEnabled = true;
        }

        //검사일자 라벨 in 체크박스 언체크시
        private void chkInspectDateSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = false;
            dtpEDate.IsEnabled = false;
            btnThisMonth.IsEnabled = false;
            btnToday.IsEnabled = false;
        }

        //금월 버튼 클릭시
        private void btnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
            dtpEDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
        }

        //금일 버튼 클릭시
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = DateTime.Today;
            dtpEDate.SelectedDate = DateTime.Today;
        }

        //작성자 라벨 클릭시
        private void lblChecker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkChecker.IsChecked == true) { chkChecker.IsChecked = false; }
            else { chkChecker.IsChecked = true; }
        }

        //작성자 라벨 in 체크박스 체크시
        private void chkChecker_Checked(object sender, RoutedEventArgs e)
        {
            txtChecker.IsEnabled = true;
            btnPfChecker.IsEnabled = true;
        }

        //작성자 라벨 in 체크박스 언체크시
        private void chkChecker_Unchecked(object sender, RoutedEventArgs e)
        {
            txtChecker.IsEnabled = false;
            btnPfChecker.IsEnabled = false;
        }

        //작성자 엔터키 이벤트용(상단)
        private void txtChecker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtChecker, 12, "");
            }
        }

        //작성자 버튼 클릭 이벤트용(상단)
        private void btnPfChecker_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtChecker, 12, "");
        }

      
        #endregion

        #region 오른 상단 버튼 동작

        //닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (MenuViewModel mvm in MainWindow.mMenulist)
            {
                if (mvm.subProgramID.ToString().Contains("MDI"))
                {
                    if (this.ToString().Equals((mvm.subProgramID as MdiChild).Content.ToString()))
                    {
                        (MainWindow.mMenulist[i].subProgramID as MdiChild).Close();
                        break;
                    }
                }
                i++;
            }
        }

        //검색
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //검색버튼 비활성화
            btnSearch.IsEnabled = false;

            Dispatcher.BeginInvoke(new Action(() =>

            {
                Thread.Sleep(2000);

                //로직
                rowNum = 0;
                re_Search(rowNum);

            }), System.Windows.Threading.DispatcherPriority.Background);



            Dispatcher.BeginInvoke(new Action(() =>

            {
                btnSearch.IsEnabled = true;

            }), System.Windows.Threading.DispatcherPriority.Background);

            


        }


        //엑셀
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] lst = new string[6];
            lst[0] = "설비점검";
            lst[1] = "설비점검 검사 범례";
            lst[2] = "설비점검 검사 수치";
            lst[3] = dgdInspect.Name;


            ExportExcelxaml ExpExc = new ExportExcelxaml(lst);
            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdInspect.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdInspect);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdInspect);

                    Name = dgdInspect.Name;
                    Lib.Instance.GenerateExcel(dt, Name);
                    Lib.Instance.excel.Visible = true;
                }

                else
                {
                    if (dt != null)
                    {
                        dt.Clear();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 재검색(수정,삭제,추가 저장후에 자동 재검색)
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void re_Search(int selectedIndex)
        {
           

            FillGrid();

            if (dgdInspect.Items.Count > 0)
            {
                dgdInspect.SelectedIndex = selectedIndex;
            }
            else
            {
                this.DataContext = null;
                dgdInspect.Items.Clear();
                dgdConfirm.Items.Clear();

                MessageBox.Show("조회된 데이터가 없습니다.");
                return;
            }
        }

        /// <summary>
        /// 실조회
        /// </summary>
        private void FillGrid()
        {
            try
            {
                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("ChkMcRInspectDate", chkInspectDateSrh.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sFromDate", chkInspectDateSrh.IsChecked == true ?
                    dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("sToDate", chkInspectDateSrh.IsChecked == true ?
                    dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("ChkMcID", chkChecker.IsChecked == true ? 1 : 0);
                sqlParameter.Add("MCID", chkChecker.IsChecked == true ? txtChecker.Tag.ToString() : "");
               
                ds = DataStore.Instance.ProcedureToDataSet("xp_McReqularInspect_sMcReqularInspect", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다.");
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            var WinMCRegulInspect = new Win_prd_HygieneCheck_CodeView()
                            {
                                Num = i + 1,
                              
                                managerid = dr["managerid"].ToString(),
                                McInsBasisDate = dr["McInsBasisDate"].ToString(),
                                McInsContent = dr["McInsContent"].ToString(),
                                BasisComments = dr["BasisComments"].ToString(),
                                McRInspectID = dr["McRInspectID"].ToString(),
                                McRInspectDate = dr["McRInspectDate"].ToString(),
                                McInsCycleGbn = dr["McInsCycleGbn"].ToString(),
                                McInsCycle = dr["McInsCycle"].ToString(),
                                Name = dr["Name"].ToString(),
                                //McRInspectUserID = dr["McRInspectUserID"].ToString(),
                                DefectContents = dr["DefectContents"].ToString(),
                                DefectReason = dr["DefectReason"].ToString(),
                                DefectRespectContents = dr["DefectRespectContents"].ToString(),
                                Comments = dr["Comments"].ToString()
                            };

                            if (WinMCRegulInspect.McInsBasisDate != null &&
                                !WinMCRegulInspect.McInsBasisDate.Replace(" ", "").Equals(""))
                            {
                                WinMCRegulInspect.McInsBasisDate_Convert =
                                //Lib.Instance.strConvertDate(WinMCRegulInspect.McInsBasisDate);
                                Lib.Instance.StrDateTimeBar(WinMCRegulInspect.McInsBasisDate);
                            }
                            if (WinMCRegulInspect.McRInspectDate != null &&
                                !WinMCRegulInspect.McRInspectDate.Replace(" ", "").Equals(""))
                            {
                                WinMCRegulInspect.McRInspectDate_Convert =
                                //Lib.Instance.strConvertDate(WinMCRegulInspect.McRInspectDate);
                                Lib.Instance.StrDateTimeBar(WinMCRegulInspect.McRInspectDate);
                            }

                            dgdInspect.Items.Add(WinMCRegulInspect);
                            i++;
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        //설비점검 메인그리드의 행 선택시
        private void dgdInspect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WinMcRegulInspect = dgdInspect.SelectedItem as Win_prd_HygieneCheck_CodeView;

            if (WinMcRegulInspect != null)
            {
                this.DataContext = WinMcRegulInspect;
                
            }
        }

    
        /// <summary>
        /// 서브 조회
        /// </summary>
        /// <param name="strID"></param>
        private void FillGridSub(string strInsID, string strBasisID, string strCycleGbn)
        {
            message = "";
            lstMsg.Clear();

            if (dgdInspect.Items.Count > 0)
            {
                dgdInspect.Items.Clear();
                dgdInspect.Refresh();
            }
            if (dgdConfirm.Items.Count > 0)
            {
                dgdConfirm.Items.Clear();
                dgdConfirm.Refresh();
            }

            try
            {
                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("McRInspectID", strInsID);
                sqlParameter.Add("McInspectBasisID", strBasisID);
                sqlParameter.Add("McInsCycleGbn", strCycleGbn);
                ds = DataStore.Instance.ProcedureToDataSet("xp_McRegularInspect_sMcRegularInspectSub", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    if (dt.Rows.Count == 0)
                    {
                        //MessageBox.Show("조회된 데이터가 없습니다.");
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            var WinMCRegulInsSub = new Win_prd_HygieneCheck_Sub_CodeView()
                            {
                                Num = i + 1,
                                McRInspectID = dr["McRInspectID"].ToString(),
                                McRSeq = dr["McRSeq"].ToString(),
                                McRInspectLegend = dr["McRInspectLegend"].ToString(),
                                McRInspectValue = stringFormatN0(dr["McRInspectValue"]),
                                McInspectBasisID = dr["McInspectBasisID"].ToString(),
                                McSeq = dr["McSeq"].ToString(),
                                McInsCheck = dr["McInsCheck"].ToString(),
                                McInsCycle = dr["McInsCycle"].ToString(),
                                McInsRecord = dr["McInsRecord"].ToString(),
                                McInsRecordGbn = dr["McInsRecordGbn"].ToString(),
                                McInsItemName = dr["McInsItemName"].ToString(),
                                McInsContent = dr["McInsContent"].ToString(),
                                McInsCycleGbn = dr["McInsCycleGbn"].ToString(),
                                Legend = dr["Legend"].ToString(),
                                McImagePath = dr["McImagePath"].ToString(),
                                McImageFile = dr["McImageFile"].ToString()
                            };

                            if (WinMCRegulInsSub.McImageFile != null && !WinMCRegulInsSub.McImageFile.Replace(" ", "").Equals(""))
                            {
                                WinMCRegulInsSub.imageFlag = true;

                                if (CheckImage(WinMCRegulInsSub.McImageFile.Trim()))
                                {
                                    string strImage = "/" + WinMCRegulInsSub.McInspectBasisID + "/" + WinMCRegulInsSub.McImageFile;
                                 
                                }
                                else
                                {
                                    MessageBox.Show(WinMCRegulInsSub.McImageFile + "는 이미지 변환이 불가능합니다.");
                                }
                            }
                            else
                            {
                                WinMCRegulInsSub.imageFlag = false;
                            }

                            ObservableCollection<CodeView> ovcMcRInspectLegend =
                                ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "MCLEGEND", "Y", "", "");

                            foreach (CodeView cv in ovcMcRInspectLegend)
                            {
                                if (cv.code_id == WinMCRegulInsSub.McRInspectLegend)
                                {
                                    WinMCRegulInsSub.LegendShape = cv.code_name;
                                    break;
                                }
                            }

                            if (WinMCRegulInsSub != null)
                            {
                                if (WinMCRegulInsSub.McInsRecordGbn.Equals("1"))
                                {
                                    dgdInspect.Items.Add(WinMCRegulInsSub);
                                }
                                else if (WinMCRegulInsSub.McInsRecordGbn.Equals("2"))
                                {
                                    dgdConfirm.Items.Add(WinMCRegulInsSub);
                                }
                            }

                            i++;
                        }
                    }

                    if (!message.Trim().Equals(""))
                    {
                        MessageBox.Show(message + " 를 불러올 수 없습니다.");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        // 확장자 이미지 확인하기, 메인윈도우에 확장자 리스트 세팅
        private bool CheckImage(string ImageName)
        {
            string[] extensions = MainWindow.Extensions;

            bool flag = false;

            ImageName = ImageName.Trim().ToLower();
            foreach (string ext in extensions)
            {
                if (ImageName.EndsWith(ext))
                {
                    flag = true;
                }
            }

            return flag;
        }

        /// <summary>
        /// 설비기준ID 와 정기검사구분으로 검사할 항목 검색
        /// </summary>
        /// <param name="strBasisID"></param>
        /// <param name="strCycleGbn"></param>
        private void FillGridSubNoResult(string strBasisID, string strCycleGbn)
        {
            message = "";
            lstMsg.Clear();

            if (dgdInspect.Items.Count > 0)
            {
                dgdInspect.Items.Clear();
            }
            if (dgdConfirm.Items.Count > 0)
            {
                dgdConfirm.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("McInspectBasisID", strBasisID);
                sqlParameter.Add("McSeq", 0);
                sqlParameter.Add("McInsCycleGbn", strCycleGbn);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_McReqularInspectBasis_sMcReqularInspectBasisSub", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            var WinSub = new Win_prd_HygieneCheck_Sub_CodeView()
                            {
                                McInsItemName = dr["McInsItemName"].ToString(),
                                McInsContent = dr["McInsContent"].ToString(),
                                McInsCheck = dr["McInsCheck"].ToString(),
                                McInsCycle = dr["McInsCycle"].ToString(),
                                McInsRecord = dr["McInsRecord"].ToString(),
                                McInspectBasisID = dr["McInspectBasisID"].ToString(),
                                McSeq = dr["McSeq"].ToString(),
                                McInsCycleGbn = dr["McInsCycleGbn"].ToString(),
                                McInsRecordGbn = dr["McInsRecordGbn"].ToString(),
                                McImagePath = dr["McImagePath"].ToString(),
                                McImageFile = dr["McImageFile"].ToString(),
                                flagBool = false
                            };

                            if (WinSub.McImageFile != null && !WinSub.McImageFile.Replace(" ", "").Equals(""))
                            {
                                WinSub.imageFlag = true;
                                
                                if (CheckImage(WinSub.McImageFile.Trim()))
                                {
                                    string strImage = "/" + WinSub.McInspectBasisID + "/" + WinSub.McImageFile;
                                }
                                else
                                {
                                    MessageBox.Show(WinSub.McImageFile + "는 이미지 변환이 불가능합니다.");
                                }
                            }
                            else
                            {
                                WinSub.imageFlag = false;
                            }

                            if (WinSub.McInsRecordGbn.Equals("1"))
                            {
                                dgdInspect.Items.Add(WinSub);
                            }

                            if (WinSub.McInsRecordGbn.Equals("2"))
                            {
                                dgdConfirm.Items.Add(WinSub);
                            }
                        }
                    }

                    if (!message.Trim().Equals(""))
                    {
                        MessageBox.Show(message + " 를 불러올 수 없습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        /// <summary>
        /// 실삭제
        /// </summary>
        /// <param name="strID"></param>
        /// <returns></returns>
        private bool DeleteData(string strID)
        {
            bool flag = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("McRInspectID", strID);

                string[] result = DataStore.Instance.ExecuteProcedure("xp_McRegularInspect_dMcRegularInspect", sqlParameter, false);

                if (result[0].Equals("success"))
                {
                    //MessageBox.Show("성공 *^^*");
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return flag;
        }


     
      
        
       

        #region 기타 메서드 모음

        // 천마리 콤마, 소수점 버리기
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }

        // 천마리 콤마, 소수점 두자리
        private string stringFormatN2(object obj)
        {
            return string.Format("{0:N2}", obj);
        }

        // 데이터피커 포맷으로 변경
        private string DatePickerFormat(string str)
        {
            string result = "";

            if (str.Length == 8)
            {
                if (!str.Trim().Equals(""))
                {
                    result = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
                }
            }

            return result;
        }

        // Int로 변환
        private int ConvertInt(string str)
        {
            int result = 0;
            int chkInt = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (Int32.TryParse(str, out chkInt) == true)
                {
                    result = Int32.Parse(str);
                }
            }

            return result;
        }

        // 소수로 변환 가능한지 체크 이벤트
        private bool CheckConvertDouble(string str)
        {
            bool flag = false;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                if (Double.TryParse(str, out chkDouble) == true)
                {
                    flag = true;
                }
            }

            return flag;
        }

        // 숫자로 변환 가능한지 체크 이벤트
        private bool CheckConvertInt(string str)
        {
            bool flag = false;
            int chkInt = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Trim().Replace(",", "");

                if (Int32.TryParse(str, out chkInt) == true)
                {
                    flag = true;
                }
            }

            return flag;
        }

        // 소수로 변환
        private double ConvertDouble(string str)
        {
            double result = 0;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (Double.TryParse(str, out chkDouble) == true)
                {
                    result = Double.Parse(str);
                }
            }

            return result;
        }

        #endregion

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgdChecker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgdConfirm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    class Win_prd_HygieneCheck_CodeView : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public string Chechker { get; set; }
        public string SanilnsItemName { get; set; }
        public string SaniInsContent { get; set; }
        public string managerid { get; set; }
        public string McInsBasisDate { get; set; }
        public string McInsContent { get; set; }
        public string BasisComments { get; set; }
        public string McRInspectID { get; set; }
        public string McRInspectDate { get; set; }
        public string McInsCycleGbn { get; set; }
        public string McInsCycle { get; set; }
        public string Name { get; set; }
        public string McRInspectUserID { get; set; }
        public string DefectContents { get; set; }
        public string DefectReason { get; set; }
        public string DefectRespectContents { get; set; }
        public string Comments { get; set; }

        //public DateTime McInsBasisDate_Convert { get; set; }
        //public DateTime McRInspectDate_Convert { get; set; }
        public string McInsBasisDate_Convert { get; set; }
        public string McRInspectDate_Convert { get; set; }
    }

    class Win_prd_HygieneCheck_Sub_CodeView : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public string McRInspectID { get; set; }
        public string McRSeq { get; set; }
        public string McRInspectLegend { get; set; }
        public string McRInspectValue { get; set; }
        public string McInspectBasisID { get; set; }
        public string McSeq { get; set; }
        public string McInsCheck { get; set; }
        public string McInsCycle { get; set; }
        public string McInsRecord { get; set; }
        public string McInsRecordGbn { get; set; }
        public string McInsItemName { get; set; }
        public string McInsContent { get; set; }
        public string McInsCycleGbn { get; set; }
        public string Legend { get; set; }
        public string McImagePath { get; set; }
        public string McImageFile { get; set; }

        public string LegendShape { get; set; }
        public bool flagBool { get; set; }
        public BitmapImage ImageView { get; set; }
        public bool imageFlag { get; set; }
    }
}
