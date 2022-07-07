using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WizMes_SeongBinFood.PopUp
{
    /// <summary>
    /// LoginPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginPage : Window
    {
        public string strLogRegID = string.Empty;

        public string PersonID = "";
        public string exPassword = "";
        public int dayDiff = 0;
        public string AccessControl = "";
        public string UserName = "";

        public string initChange = "Y";

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetInfo();
        }

        //로그인
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Log(txtUserID.Text))
            {
                strLogRegID = txtUserID.Text;
                Lib.Instance.SetLogResitry(strLogRegID);
                DialogResult = true;
            }
            else
            {
                txtPassWd.Password = "";
                return;
            }
        }

        //취소
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private bool Log(string strID)
        {
            #region 20210823 암호화 이전의 소스
            //bool flag = true;

            //DataSet ds = null;
            //Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            //sqlParameter.Clear();
            //sqlParameter.Add("UserID", strID);
            //ds = DataStore.Instance.ProcedureToDataSet("xp_Common_Login", sqlParameter, false);


            //if (ds != null && ds.Tables.Count > 0)
            //{
            //    DataTable dt = ds.Tables[0];

            //    if (dt.Rows.Count <= 0)
            //    {
            //        MessageBox.Show("존재하지 않는 ID 입니다.");
            //        flag = false;
            //        return flag;
            //    }
            //    else
            //    {
            //        if (!dt.Rows[0]["Password"].ToString().Equals(txtPassWd.Password))
            //        {
            //            MessageBox.Show("비밀번호가 잘못되었습니다.");
            //            flag = false;
            //            return flag;
            //        }

            //        //if (!dt.Rows[0]["Name"].Equals("20150401") && !dt.Rows[0]["Name"].Equals("admin"))
            //        //{
            //        //    MessageBox.Show("권한이 없는 사용자입니다.");
            //        //    return flag;
            //        //}

            //        // 비밀번호 변경 추가
            //        if (CheckConvertDateTime(dt.Rows[0]["PasswordChangeDate"].ToString()) == true)
            //        {
            //            DateTime setDate = DateTime.Parse(dt.Rows[0]["PasswordChangeDate"].ToString());

            //            TimeSpan timeDiff = DateTime.Today - setDate;
            //            dayDiff = timeDiff.Days;

            //            if (dayDiff > 90)
            //            {
            //                exPassword = dt.Rows[0]["Password"].ToString();
            //            }
            //        }
            //        else if (dt.Rows[0]["PasswordChangeDate"].ToString().Trim().Equals("")) // 초기 비밀번호가 세팅되지 않았다면
            //        {
            //            initChange = "N";
            //            exPassword = dt.Rows[0]["Password"].ToString();
            //        }

            //        // 개인정보활용 동의 여부 추가, 이름도 추가
            //        AccessControl = dt.Rows[0]["NessaryAcptYN"].ToString();
            //        UserName = dt.Rows[0]["Name"].ToString();

            //        //PersonID
            //        PersonID = dt.Rows[0]["PersonID"].ToString();
            //    }
            //}

            //return flag;
            #endregion

            bool flag = true;

            DataSet ds = null;
            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Clear();
            sqlParameter.Add("UserID", strID);
            sqlParameter.Add("Password", txtPassWd.Password);
            ds = DataStore.Instance.ProcedureToDataSet("xp_Common_Login", sqlParameter, false);


            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    if (dt.Columns.Count == 1)
                    {
                        MessageBox.Show(dt.Rows[0].ItemArray[0].ToString());
                        return false;
                    }
                    else
                    {
                        //if (!dt.Rows[0]["Name"].Equals("20150401") && !dt.Rows[0]["Name"].Equals("admin"))
                        //{
                        //    MessageBox.Show("권한이 없는 사용자입니다.");
                        //    return flag;
                        //}

                        // 비밀번호 변경 추가
                        if (CheckConvertDateTime(dt.Rows[0]["PasswordChangeDate"].ToString()) == true)
                        {
                            DateTime setDate = DateTime.Parse(dt.Rows[0]["PasswordChangeDate"].ToString());

                            TimeSpan timeDiff = DateTime.Today - setDate;
                            dayDiff = timeDiff.Days;

                            if (dayDiff > 90)
                            {
                                //exPassword = dt.Rows[0]["Password"].ToString();
                            }
                        }
                        else if (dt.Rows[0]["PasswordChangeDate"].ToString().Trim().Equals("")) // 초기 비밀번호가 세팅되지 않았다면
                        {
                            initChange = "N";
                            //exPassword = dt.Rows[0]["Password"].ToString();
                        }

                        // 개인정보활용 동의 여부 추가, 이름도 추가
                        AccessControl = dt.Rows[0]["NessaryAcptYN"].ToString();
                        UserName = dt.Rows[0]["Name"].ToString();

                        //PersonID
                        PersonID = dt.Rows[0]["PersonID"].ToString();
                    }
                }
            }
            DataStore.Instance.CloseConnection(); //2021-09-13 현달씨 DBClose

            return flag;

        }


        private void GetInfo()
        {
            txtUserID.Text = Lib.Instance.GetLogResitry();

            if (txtUserID.Text.Equals(""))
            {
                txtUserID.Focus();
            }
            else
            {
                txtPassWd.Focus();
            }
        }

        private void txtPassWd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Log(txtUserID.Text))
                {
                    strLogRegID = txtUserID.Text;
                    Lib.Instance.SetLogResitry(strLogRegID);
                    DialogResult = true;
                }
                else
                {
                    txtPassWd.Password = "";
                    return;
                }
            }
        }

        #region 기타 메서드

        // 천마리 콤마, 소수점 버리기
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }

        // DateTime 으로 변환 가능한지 체크
        private bool CheckConvertDateTime(string str)
        {
            bool flag = false;

            DateTime chkDt;

            if (!str.Trim().Equals(""))
            {
                if (str.Length == 8)
                {
                    str = DatePickerFormat(str);

                    if (DateTime.TryParse(str, out chkDt) == true)
                    {
                        flag = true;
                        return flag;
                    }
                }
                else
                {
                    if (DateTime.TryParse(str, out chkDt) == true)
                    {
                        flag = true;
                        return flag;
                    }
                }
            }

            return flag;
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


        #endregion // 기타 메서드
    }
}
