﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace PunchForce
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            String username = nameTextBox.Text.ToString();
            HttpClient httpClientBase = login("yL3P/tHg", "21218CCA77804D2BA1922C33E0151105");
            try {
                String base64Name = getBase64Name(username, httpClientBase);
                if (base64Name.Equals("s8K8yraw"))
                {
                    MessageBox.Show("放下那只鼠标让本宝宝自己来~");
                    return;
                }
                emp emp = getPasswdAndId(username, httpClientBase);
                emp.EmpName = username;
                emp.Bs64Name = base64Name;
                HttpClient httpClient = login(emp.Bs64Name, emp.Passwd);
                job job = getJobDate(httpClient);
                String jobSql = getJobSql(emp, job);
                String response = injectJobData(httpClient, jobSql);
                MessageBox.Show("强制" + job.Project+"项目组"+username + "报工成功！");
            }
            catch (Exception exception) {
                MessageBox.Show("报工失败：名字有误OR已冻结OR LEADER！");
            }
        }
        //登录获取httpclient
        public HttpClient login(String username,String password)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:48.0) Gecko/20100101 Firefox/48.0");
            String loginUrl = "http://123.232.10.234:8083/servlet/com.sdjxd.pms.platform.serviceBreak.Invoke";
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("_c", "com.sdjxd.pms.platform.organize.User"));
            paramList.Add(new KeyValuePair<string, string>("_m", "loginByEncode"));
            paramList.Add(new KeyValuePair<string, string>("_p0", username));
            paramList.Add(new KeyValuePair<string, string>("_p1", password));
            HttpResponseMessage response = httpClient.PostAsync(new Uri(loginUrl), new FormUrlEncodedContent(paramList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            return httpClient;
        }
        //获取加密名字
        public String getBase64Name(String username, HttpClient httpClient)
        {
            String Base64URL = "http://123.232.10.234:8083/servlet/com.sdjxd.pms.platform.serviceBreak.Invoke";
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("_c", "com.sdjxd.pms.platform.tool.Base64"));
            paramList.Add(new KeyValuePair<string, string>("_m", "encode"));
            paramList.Add(new KeyValuePair<string, string>("_p0", username));
            HttpResponseMessage response = httpClient.PostAsync(new Uri(Base64URL), new FormUrlEncodedContent(paramList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            Regex regex = new Regex("\"value\":\"");
            String[] str = regex.Split(result);
            if (str.Length < 2) return null;
            Regex regex64 = new Regex("\"},\"com");
            String[] base64Name = regex64.Split(str[1]);
            return base64Name[0];

        }
        //获取ID密码
        public emp getPasswdAndId(String username,HttpClient httpClient)
        {
            emp emp = new emp();
            String sqlurl = "http://123.232.10.234:8083/servlet/com.sdjxd.pms.platform.serviceBreak.Invoke?p=6962531A-0F5E-43E9-84ED-185AE9A93CFE";
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("_c", "com.sdjxd.pms.platform.form.service.cell.ComboBox"));
            paramList.Add(new KeyValuePair<string, string>("_m", "refresh"));
            paramList.Add(new KeyValuePair<string, string>("_p0", "\"defaultds\""));
            paramList.Add(new KeyValuePair<string, string>("_p1", "\"[\"2\",[\"JXD7_XT_USER\",\"USERID\",\"PASSWD\",\" WHERE 1=1 AND USERNAME = '" + username + "'\",\" ORDER BY USERID\"],\"0\",\"0\",\"0\",\"1\"]\""));
            paramList.Add(new KeyValuePair<string, string>("_p2", "\"6962531A-0F5E-43E9-84ED-185AE9A93CFE\""));
            paramList.Add(new KeyValuePair<string, string>("_p3", "77"));
            HttpResponseMessage response = httpClient.PostAsync(new Uri(sqlurl), new FormUrlEncodedContent(paramList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            Regex regex = new Regex("JSClass.extend\\(\\[\"");
            String[] str = regex.Split(result);
            if (str.Length < 3) return null;
            Regex regexId = new Regex("\"],\"");
            String[] objectId = regexId.Split(str[1]);
            String[] password = regexId.Split(str[2]);
            emp.ObjectId = objectId[0];
            emp.Passwd = password[0];
            return emp;
        }
        //封装报工实体
        public job getJobDate(HttpClient httpClient)
        {
            job job = new job();
            String joburl = "http://123.232.10.234:8083/servlet/com.sdjxd.pms.platform.serviceBreak.Invoke?p=6962531A-0F5E-43E9-84ED-185AE9A93CFE";
            var sheetId = Guid.NewGuid().ToString();
            job.SheetId = sheetId;
            DateTime dt = System.DateTime.Now;
            String date = dt.ToString("yyyy-MM-dd");
            String datejq = dt.ToString("yyyy-MM-dd HH:mm:ss");
            job.Data = date;
            job.Datajq = date;
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("_c", "bgxt.BgxtPc"));
            paramList.Add(new KeyValuePair<string, string>("_m", "getData"));
            HttpResponseMessage response = httpClient.PostAsync(new Uri(joburl), new FormUrlEncodedContent(paramList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            String[] str1 = new Regex("JSClass.extend\\(\\[").Split(result);
            if (str1.Length < 2) return null;
            String[] str2 = new Regex("],\"").Split(str1[1]);
            if (str2.Length < 2) return null;
            String[] jobstr = new Regex(",").Split(str2[0]);
            if (jobstr.Length > 0)
            {
                job.Place = jobstr[0].Replace("\"", "");
                job.Project = jobstr[1].Replace("\"", "");
                job.Quarter = jobstr[2].Replace("\"", "");
                job.Desc = jobstr[3].Replace("\"", "");
                job.PlaceId = jobstr[4].Replace("\"", "");
                job.ProjectId = jobstr[5].Replace("\"", "");
                job.QuarterId = jobstr[6].Replace("\"", "");
                job.Bgrq = jobstr[7].Replace("\"", "");
                job.PcType = jobstr[8].Replace("\"", "");
                job.WorkType = jobstr[9].Replace("\"", "");
            }

            return job;
        }
        //拼接注入SQL
        public String getJobSql(emp emp,job job)
        {
            StringBuilder jobSQL = new StringBuilder();
            jobSQL.Append("     IF NOT EXISTS (SELECT * FROM dbo.BGXT_BGLRB WHERE CREATEUSERID='" + emp.ObjectId + "' AND BGRQ = '" + job.Data + "') ");
            jobSQL.Append("     BEGIN ");
            jobSQL.Append("  	INSERT INTO dbo.BGXT_BGLRB (	  ");
            jobSQL.Append("  	BEIZHU,	  ");
            jobSQL.Append("  	BGRQ,	  ");
            jobSQL.Append("  	BGSTATUS,	  ");
            jobSQL.Append("  	CQQK,	  ");
            jobSQL.Append("  	CREATEDATE,	  ");
            jobSQL.Append("  	CREATEDEPT,	  ");
            jobSQL.Append("  	CREATEDEPTID,	  ");
            jobSQL.Append("  	CREATEORG,	  ");
            jobSQL.Append("  	CREATEORGID,	  ");
            jobSQL.Append("  	CREATEUSER,	  ");
            jobSQL.Append("  	CREATEUSERID,	  ");
            jobSQL.Append("  	DATASTATUSID,	  ");
            jobSQL.Append("  	DNSY,	  ");
            jobSQL.Append("  	EDITUSER,	  ");
            jobSQL.Append("  	EDITUSERID,	  ");
            jobSQL.Append("  	GZDID,	  ");
            jobSQL.Append("  	GZDMC,	  ");
            jobSQL.Append("  	GZL,	  ");
            jobSQL.Append("  	JBSJ,	  ");
            jobSQL.Append("  	LASTOPENTIME,	  ");
            jobSQL.Append("  	LOCATION,	  ");
            jobSQL.Append("  	OPENER,	  ");
            jobSQL.Append("  	OPENERID,	  ");
            jobSQL.Append("  	PATTERNID,	  ");
            jobSQL.Append("  	SBLX,	  ");
            jobSQL.Append("  	SHEETID,	  ");
            jobSQL.Append("  	SHEETNAME,	  ");
            jobSQL.Append("  	SHOWORDER,	  ");
            jobSQL.Append("  	SHZT,	  ");
            jobSQL.Append("  	XMZID,	  ");
            jobSQL.Append("  	XMZMC,	  ");
            jobSQL.Append("  	YXMZID,	  ");
            jobSQL.Append("  	YXMZMC,	  ");
            jobSQL.Append("  	YZSFWID,	  ");
            jobSQL.Append("  	YZSFWMC,	  ");
            jobSQL.Append("  	ZSFWID,	  ");
            jobSQL.Append("  	ZSFWMC	  ");
            jobSQL.Append("  	) SELECT	  ");
            jobSQL.Append("  	BEIZHU,	  ");
            jobSQL.Append("  	'" + job.Data + "',	  ");
            jobSQL.Append("  	'2',	  ");
            jobSQL.Append("  	CQQK,	  ");
            jobSQL.Append("  	'" + job.Datajq + "',	  ");
            jobSQL.Append("  	CREATEDEPT,	  ");
            jobSQL.Append("  	CREATEDEPTID,	  ");
            jobSQL.Append("  	CREATEORG,	  ");
            jobSQL.Append("  	CREATEORGID,	  ");
            jobSQL.Append("  	CREATEUSER,	  ");
            jobSQL.Append("  	CREATEUSERID,	  ");
            jobSQL.Append("  	DATASTATUSID,	  ");
            jobSQL.Append("  	DNSY,	  ");
            jobSQL.Append("  	EDITUSER,	  ");
            jobSQL.Append("  	EDITUSERID,	  ");
            jobSQL.Append("  	GZDID,	  ");
            jobSQL.Append("  	GZDMC,	  ");
            jobSQL.Append("  	GZL,	  ");
            jobSQL.Append("  	JBSJ,	  ");
            jobSQL.Append("  	'" + job.Datajq + "',	  ");
            jobSQL.Append("  	LOCATION,	  ");
            jobSQL.Append("  	OPENER,	  ");
            jobSQL.Append("  	OPENERID,	  ");
            jobSQL.Append("  	PATTERNID,	  ");
            jobSQL.Append("  	SBLX,	  ");
            jobSQL.Append("  	'" + job.SheetId + "',	  ");
            jobSQL.Append("  	SHEETNAME,	  ");
            jobSQL.Append("  	SHOWORDER,	  ");
            jobSQL.Append("  	SHZT,	  ");
            jobSQL.Append("  	XMZID,	  ");
            jobSQL.Append("  	XMZMC,	  ");
            jobSQL.Append("  	YXMZID,	  ");
            jobSQL.Append("  	YXMZMC,	  ");
            jobSQL.Append("  	YZSFWID,	  ");
            jobSQL.Append("  	YZSFWMC,	  ");
            jobSQL.Append("  	ZSFWID,	  ");
            jobSQL.Append("  	ZSFWMC	  ");
            jobSQL.Append("  	FROM	  ");
            jobSQL.Append("  	dbo.BGXT_BGLRB	  ");
            jobSQL.Append("  	WHERE	  ");
            jobSQL.Append("  	CREATEUSERID = '" + emp.ObjectId + "'	  ");
            jobSQL.Append("  	AND BGRQ = '" + job.Bgrq + "'	  ");
            jobSQL.Append("     END ");
            return jobSQL.ToString();
        }
        //注入报工
        public String injectJobData(HttpClient httpClient,String jobSql)
        {
            String injectUrl = "http://123.232.10.234:8083/servlet/com.sdjxd.pms.platform.serviceBreak.Invoke?p=6962531A-0F5E-43E9-84ED-185AE9A93CFE";
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("_c", "com.sdjxd.pms.platform.form.service.cell.ComboBox"));
            paramList.Add(new KeyValuePair<string, string>("_m", "refresh"));
            paramList.Add(new KeyValuePair<string, string>("_p0", "\"defaultds\""));
            paramList.Add(new KeyValuePair<string, string>("_p1", "\"[\\\"2\\\",[\\\"JXD7_XT_USER\\\",\\\"USERNAME\\\",\\\"USERID\\\",\\\" WHERE 1=1 AND USERID = '6962531A-0F5E-43E9-84ED-185AE9A93CFE'\\\",\\\" ORDER BY USERID;" + jobSql + "\\\"],\\\"0\\\",\\\"0\\\",\\\"0\\\",\\\"1\\\"]\""));
            paramList.Add(new KeyValuePair<string, string>("_p2", "\"6962531A-0F5E-43E9-84ED-185AE9A93CFE\""));
            paramList.Add(new KeyValuePair<string, string>("_p3", "77"));
            HttpResponseMessage response = httpClient.PostAsync(new Uri(injectUrl), new FormUrlEncodedContent(paramList)).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
    }
}
