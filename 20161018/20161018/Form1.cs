using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
//using Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
//using Excel = Microsoft.Office.Interop.Excel;
//using NPOI.XSSF.UserModel;
using OfficeOpenXml;

namespace _20161018
{
    public partial class Form1 : Form
    {
        private System.Data.DataTable dtOld = new System.Data.DataTable("Old");
        private System.Data.DataTable dtNew = new System.Data.DataTable("New");
        private String inPath;
        private String extensionName;
        private String fileName;
        private String kindName;
        private String methodName = "";
        private Thread thread;
        private Thread proBarThread;
        private DateTime startTime;
        private DateTime endTime;
        TimeSpan subTime;

        private int nodes_HD_Num;
        private String[] nodes_HD;
        private Dictionary<String, int> weight_CR_HD = new Dictionary<string, int>();
        private Dictionary<String, int> weight_ET_HD = new Dictionary<string, int>();
        private Dictionary<String, int> weight_YE_HD = new Dictionary<string, int>();
        private Dictionary<String, int> weight_XL_HD = new Dictionary<string, int>();
        private Dictionary<String, int> weight_YJ_HD = new Dictionary<string, int>();
        private Dictionary<String, int> weight_HW_HD = new Dictionary<string, int>();
        private int allLine;
        private int currentLine;
        private String HX;


        private delegate void SetPos(int ipos, string vinfo);


        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 打开文件，获取文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //点击弹出打开文件对话框
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "请选择要打开的文件";
            ofd.Filter = "DBF文件|*.dbf|Excel 2007/2010文件|*.xlsx";
            ofd.ShowDialog();

            //获得在打开对话框中选中文件的路径
            inPath = ofd.FileName;

            if (inPath == "")
            {
                return;
            }
            txt_showPath.Text = inPath;

            fileName = Path.GetFileNameWithoutExtension(inPath);
            extensionName = Path.GetExtension(inPath);
            StringBuilder sb = new StringBuilder();
            sb.Append(fileName[0] + fileName[1]);
            kindName = sb.ToString();

            if (methodName != "")
            {
                btn_convertToDbf.Enabled = true;
                btn_convertToXlsx.Enabled = true;
            }

        }




        /// <summary>
        /// 转换按钮点击触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_convert_Click(object sender, EventArgs e)
        {
            txtTest.Text = "";
            timer1.Enabled = true;
            btn_stop.Enabled = true;

            thread = new Thread(initDbf);
            thread.IsBackground = true;
            thread.Start();
        }


        /// <summary>
        /// DBF的运行主函数
        /// </summary>
        private void initDbf()
        {
            startTime = DateTime.Now;

            btn_convertToXlsx.Enabled = false;
            btn_inputFile.Enabled = false;
            btn_convertToDbf.Enabled = false;
            radioBtnYY.Enabled = false;
            radioBtnYT.Enabled = false;
            radioBtnYF.Enabled = false;

            //写入DBF
            inputDBF();

            txtTest.AppendText(dtOld.Rows.Count.ToString() + " " + dtOld.Columns.Count.ToString() + "\n");
            if (dtOld != null && dtOld.Rows.Count > 0)
            {
                dtNew = dtOld.Copy();
                dtNew.TableName = fileName;
                if (dtNew.Columns["date"] != null)
                {
                    dtNew.Columns["DATE"].ColumnName = "DATE1";
                    dtNew.Columns["DATE1"].SetOrdinal(0);
                }
                if (methodName == "YY")
                {
                    init();
                    doWithInitHD();
                    doWithDelete();
                    doWithAdd();
                    doWithCODE_SHARE();
                    doWithHD();
                    outputDBF();
                }
                else if (methodName == "YT")
                {
                    doWithDelete();
                    doWithAdd();
                    outputDBF();
                }
                else
                {
                    init();
                    outputDBF();
                }
            }
            btn_convertToXlsx.Enabled = true;
            btn_inputFile.Enabled = true;
            btn_convertToDbf.Enabled = true;
            btn_stop.Enabled = false;
            timer1.Enabled = false;
            radioBtnYF.Enabled = true;
            radioBtnYT.Enabled = true;
            radioBtnYY.Enabled = true;
            radioBtnYF.Checked = false;
            radioBtnYT.Checked = false;
            radioBtnYY.Checked = false;
            txtTest.AppendText("over");
            MessageBox.Show("数据转换完毕","提示");
        }


        /// <summary>
        /// 将DBF文件一次性全部写入内存并创建DataTable
        /// </summary>
        private void inputDBF()
        {
            //文件路径
            string filePath = inPath;
            FileInfo fileInfo = new FileInfo(filePath);
            string directory = fileInfo.DirectoryName;
            string fileName = fileInfo.Name;

            OleDbConnection conn = new OleDbConnection();
            string table = filePath;

            string connStr = @"Provider=VFPOLEDB.1;Data Source=" + directory + ";Collating Sequence=MACHINE";

            conn.ConnectionString = connStr;
            conn.Open();

            string sql = @"select * from " + fileName;
            OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);

            da.Fill(dtOld);
        }


        private void init()
        {
            if (methodName == "YY")
            {
                foreach (DataRow dr in dtNew.Rows)
                {
                    dr["FXD"] = dr["FXD"].ToString() + dr["DW"].ToString();
                    dr["JH"] = "B-" + dr["JH"].ToString();
                }
            }
            else if (methodName == "YT")
            {
                foreach (DataRow dr in dtNew.Rows)
                {
                    dr["FXD"] = dr["FXD"].ToString() + dr["DW"].ToString();
                    dr["JH"] = "B-" + dr["JH"].ToString();
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// 新增需求中的字段
        /// </summary>
        private void doWithAdd()
        {
            if (methodName == "YY")
            {
                //新增字段
                if (dtNew.Columns["CODE_SHARE"] == null)
                    dtNew.Columns.Add("CODE_SHARE", System.Type.GetType("System.String"));
                if (dtNew.Columns["R_HDJL"] == null)
                    dtNew.Columns.Add("R_HDJL", System.Type.GetType("System.String"));
                //if (dtNew.Columns["APU_TIME"] == null)
                //    dtNew.Columns.Add("APU_TIME", System.Type.GetType("System.String"));
                if (dtNew.Columns["HXSK"] == null)
                    dtNew.Columns.Add("HXSK", System.Type.GetType("System.String"));
                if (dtNew.Columns["APUSJ"] == null)
                    dtNew.Columns.Add("APUSJ", System.Type.GetType("System.String"));


                //新增各种航段
                if (dtNew.Columns["CR_HD"] == null)
                    dtNew.Columns.Add("CR_HD");
                if (dtNew.Columns["ET_HD"] == null)
                    dtNew.Columns.Add("ET_HD");
                if (dtNew.Columns["YE_HD"] == null)
                    dtNew.Columns.Add("YE_HD");
                if (dtNew.Columns["XL_HD"] == null)
                    dtNew.Columns.Add("XL_HD");
                if (dtNew.Columns["YJ_HD"] == null)
                    dtNew.Columns.Add("YJ_HD");
                if (dtNew.Columns["HW_HD"] == null)
                    dtNew.Columns.Add("HW_HD");

                if (dtNew.Columns["BJ"] == null)
                    dtNew.Columns.Add("BJ").DefaultValue = "0";
                if (dtNew.Columns["BJD"] == null)
                    dtNew.Columns.Add("BJD");
            }
            else if (methodName == "YT")
            {
                if (dtNew.Columns["HBH"] == null)
                    dtNew.Columns.Add("HBH");
                if (dtNew.Columns["CLDSK"] == null)
                    dtNew.Columns.Add("CLDSK");
                if (dtNew.Columns["HXSK"] == null)
                    dtNew.Columns.Add("HXSK");
                if (dtNew.Columns["QFSK"] == null)
                    dtNew.Columns.Add("QFSK");
                if (dtNew.Columns["JLSK"] == null)
                    dtNew.Columns.Add("JLSK");
                if (dtNew.Columns["SLDSK"] == null)
                    dtNew.Columns.Add("SLDSK");
                if (dtNew.Columns["APUSJ"] == null)
                    dtNew.Columns.Add("APUSJ");
            }
            else
            {

            }
        }



        /// <summary>
        /// 删除需求中的字段
        /// </summary>
        private void doWithDelete()
        {
            if (methodName == "YY")
            {
                if (dtNew.Columns["HX0"] != null)
                    dtNew.Columns.Remove("HX0");
                if (dtNew.Columns["XSF"] != null)
                    dtNew.Columns.Remove("XSF");
            }
            else if (methodName == "YT")
            {
                if (dtNew.Columns["HX0"] != null)
                    dtNew.Columns.Remove("HX0");
            }
            else if (methodName == "YF")
            {

            }
        }



        /// <summary>
        /// 处理CODE_SHARE字段
        /// </summary>
        /// <param name="codeShare">三个CODE_SHARE输入字段数组</param>
        /// <returns>返回处理完毕的CODE_SHARE</returns>
        private void doWithCODE_SHARE()
        {
            Regex regex = new Regex(@"CODE_SHARE(\d*)");
            String line;
            int max = 0;
            int value = 0;

            for (int i = 0; i < dtNew.Columns.Count; i++)
            {
                line = dtNew.Columns[i].ColumnName;
                Match match = regex.Match(line);
                if (match.Groups[1].Value != "")
                    value = Convert.ToInt32(match.Groups[1].Value);
                if (value > max)
                    max = value;
            }

            if (max == 0)
                return;

            int flag = 1;
            String gridValue;
            DataRow drOperate = dtNew.Rows[0];
            StringBuilder sbCreate = new StringBuilder();

            for (int i = 0; i < dtNew.Rows.Count; i++)
            {
                drOperate = dtNew.Rows[i];
                for (int j = 1; j <= max; j++)
                {
                    gridValue = drOperate["CODE_SHARE" + j].ToString();
                    if (gridValue != "")
                    {
                        if (flag == 1)
                        {
                            sbCreate.Append(gridValue);
                            flag = 0;
                        }
                        else sbCreate.Append("," + gridValue);
                    }
                }
            }
            drOperate["CODE_SHARE"] = sbCreate.ToString();
        }



        /// <summary>
        /// 为HD的计算初始化
        /// </summary>
        private void doWithInitHD()
        {
            //排序的新添字段
            dtNew.Columns.Add("DATE_HBH_DW_HX_PBM");
            DataRow drOperate;
            StringBuilder sbCreate = new StringBuilder();
            sbCreate.Clear();
            for (int i = 0; i < dtNew.Rows.Count; i++)
            {
                sbCreate.Clear();
                drOperate = dtNew.Rows[i];
                if (extensionName == ".xlsx")
                    sbCreate.Append(drOperate["date"].ToString().Trim() + " ");
                else sbCreate.Append(drOperate["date1"].ToString().Trim() + " ");
                sbCreate.Append(drOperate["hbh"].ToString().Trim() + " ");
                sbCreate.Append(drOperate["dw"].ToString().Trim() + " ");
                sbCreate.Append(drOperate["hx"].ToString().Trim() + " ");
                sbCreate.Append(drOperate["pbm"].ToString().Trim() + " ");
                drOperate["DATE_HBH_DW_HX_PBM"] = sbCreate.ToString();

            }

            DataView dv = dtNew.DefaultView;
            dv.Sort = "DATE_HBH_DW_HX_PBM";
            dtNew = dv.ToTable();

            #region 显示排序列 DATE_HBH_DW_HX_PBM
            //for (int i = 0; i < dtNew.Rows.Count; i++)
            //{
            //    drOperate = dtNew.Rows[i];
            //    txtTest.AppendText(drOperate["DATE_HBH_DW_HX_PBM"] + " ");
            //}
            #endregion

        }


        /// <summary>
        /// 初始化航段所需的条件，添加一列处理四个字段使相同
        /// </summary>
        private void doWithHD()
        {
            try
            {
                currentLine = 0;
                allLine = 0;
                weight_CR_HD.Clear();
                weight_ET_HD.Clear();
                weight_YE_HD.Clear();
                weight_XL_HD.Clear();
                weight_YJ_HD.Clear();
                weight_HW_HD.Clear();
                DataRow drOperate;
                //回滚用临时行数
                DataRow drOperate2;

                //行循环
                for (int i = 0; i < dtNew.Rows.Count; i++)
                {
                    drOperate = dtNew.Rows[i];
                    //txtTest.AppendText("外1+ 行数=" + i + "HBH" + drOperate["hbh"] + "\n");
                    currentLine++;
                    drOperate["CR_HD"] = 0;
                    drOperate["ET_HD"] = 0;
                    drOperate["YE_HD"] = 0;
                    drOperate["XL_HD"] = 0;
                    drOperate["YJ_HD"] = 0;
                    drOperate["HW_HD"] = 0;

                    drOperate["BJ"] = 0;
                    //多行，当前中间
                    if (allLine > 1 && allLine != currentLine)
                    {
                        if (drOperate["hx"].ToString().Trim() != HX)
                        {
                            currentLine = 0;
                            allLine = 0;
                            weight_CR_HD.Clear();
                            weight_ET_HD.Clear();
                            weight_YE_HD.Clear();
                            weight_XL_HD.Clear();
                            weight_YJ_HD.Clear();
                            weight_HW_HD.Clear();
                            continue;
                        }

                        //txtTest.AppendText("多行，当前中间2+ 行数=" + i + "HBH " + drOperate["hbh"] + "\n");

                        //txtTest.AppendText("miao" + i + "\n");
                        //txtTest.AppendText(drOperate["HD"].ToString().Trim() + Convert.ToInt32((drOperate["CR"]).ToString()));
                        //foreach (var item in weight_CR_HD)
                        //{
                        //    txtTest.AppendText(item.Key + " " + item.Value + " hou ");
                        //}
                        if (weight_CR_HD.ContainsKey(drOperate["HD"].ToString().Trim()))
                        {

                            i += nodes_HD_Num - currentLine;
                            weight_CR_HD.Clear();
                            weight_ET_HD.Clear();
                            weight_YE_HD.Clear();
                            weight_XL_HD.Clear();
                            weight_YJ_HD.Clear();
                            weight_HW_HD.Clear();
                            txtTest.AppendText(allLine.ToString());
                            for (int k = 0; k < allLine; k++)
                            {

                                dtNew.Rows.Remove(dtNew.Rows[i - currentLine + 1]);
                            }
                            currentLine = 0;
                            allLine = 0;
                            i -= currentLine;
                            continue;
                        }
                        else
                        {
                            weight_CR_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["CR"]).ToString()));
                        }
                        weight_ET_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["ET"]).ToString()));
                        //txtTest.AppendText("2.3\n");
                        weight_YE_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["YE"]).ToString()));
                        //txtTest.AppendText("2.4\n");
                        weight_XL_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["XL"]).ToString()));
                        //txtTest.AppendText("2.5\n");
                        weight_YJ_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["YJ"]).ToString()));
                        //txtTest.AppendText("2.6\n");
                        weight_HW_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["HW"]).ToString()));
                        //txtTest.AppendText("2.7\n");
                    }
                    //多行，当前最后
                    else if (allLine > 1 && allLine == currentLine)
                    {
                        try
                        {
                            if (drOperate["hx"].ToString().Trim() != HX)
                            {
                                currentLine = 0;
                                allLine = 0;
                                weight_CR_HD.Clear();
                                weight_ET_HD.Clear();
                                weight_YE_HD.Clear();
                                weight_XL_HD.Clear();
                                weight_YJ_HD.Clear();
                                weight_HW_HD.Clear();
                                continue;
                            }

                            //txtTest.AppendText("多行，当前最后3+ 行数=" + i + "HBH" + drOperate["hbh"] + "\n");

                            weight_CR_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["CR"]).ToString()));
                            weight_ET_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["ET"]).ToString()));
                            weight_YE_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["YE"]).ToString()));
                            weight_XL_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["XL"]).ToString()));
                            weight_YJ_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["YJ"]).ToString()));
                            weight_HW_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["HW"]).ToString()));


                            for (int m = 0; m < nodes_HD.Length; m++)
                            {
                                nodes_HD[m] = nodes_HD[m].Trim();
                            }

                            Dictionary<String, int> tempDic_CR = doWithHD(nodes_HD, weight_CR_HD);
                            Dictionary<String, int> tempDic_ET = doWithHD(nodes_HD, weight_ET_HD);
                            Dictionary<String, int> tempDic_YE = doWithHD(nodes_HD, weight_YE_HD);
                            Dictionary<String, int> tempDic_XL = doWithHD(nodes_HD, weight_XL_HD);
                            Dictionary<String, int> tempDic_YJ = doWithHD(nodes_HD, weight_YJ_HD);
                            Dictionary<String, int> tempDic_HW = doWithHD(nodes_HD, weight_HW_HD);

                            for (int j = 0; j < nodes_HD_Num; j++)
                            {
                                drOperate2 = dtNew.Rows[i - j];

                                drOperate2["CR_HD"] = tempDic_CR[drOperate2["HD"].ToString().Trim()];
                                drOperate2["ET_HD"] = tempDic_ET[drOperate2["HD"].ToString().Trim()];
                                drOperate2["YE_HD"] = tempDic_YE[drOperate2["HD"].ToString().Trim()];
                                drOperate2["XL_HD"] = tempDic_XL[drOperate2["HD"].ToString().Trim()];
                                drOperate2["YJ_HD"] = tempDic_YJ[drOperate2["HD"].ToString().Trim()];
                                drOperate2["HW_HD"] = tempDic_HW[drOperate2["HD"].ToString().Trim()];
                            }
                            allLine = 0;
                            currentLine = 0;
                            weight_CR_HD.Clear();
                            weight_ET_HD.Clear();
                            weight_YE_HD.Clear();
                            weight_XL_HD.Clear();
                            weight_YJ_HD.Clear();
                            weight_HW_HD.Clear();

                        }
                        catch (Exception ex)
                        {
                            txtTest.AppendText("多行的最后一行：" + ex.Message + "\n");
                        }
                    }
                    //单行或多行当前第一
                    else
                    {
                        //txtTest.AppendText("外4+ 行数=" + i + "HBH" + drOperate["hbh"] + "\n");
                        HX = drOperate["HX"].ToString().Trim();
                        nodes_HD = HX.Split('-');
                        nodes_HD_Num = nodes_HD.Length;


                        //单行
                        if (nodes_HD_Num == 2)
                        {
                            //txtTest.AppendText("单行5+ 行数=" + i + "HBH" + drOperate["hbh"] + "\n");

                            drOperate["CR_HD"] = drOperate["CR"];
                            drOperate["ET_HD"] = drOperate["ET"];
                            drOperate["YE_HD"] = drOperate["YE"];
                            drOperate["XL_HD"] = drOperate["XL"];
                            drOperate["YJ_HD"] = drOperate["YJ"];
                            drOperate["HW_HD"] = drOperate["HW"];
                            currentLine = 0;
                            allLine = 0;
                        }
                        //多行，当前第一
                        else
                        {
                            //txtTest.AppendText("多行，当前第一6+ 行数=" + i + "HBH" + drOperate["hbh"] + "\n");

                            allLine = nodes_HD_Num * (nodes_HD_Num - 1) / 2;

                            //txtTest.AppendText("miao" + i + "\n");
                            //txtTest.AppendText(drOperate["HD"].ToString().Trim() + Convert.ToInt32((drOperate["CR"]).ToString()));
                            //foreach (var item in weight_CR_HD)
                            //{
                            //    txtTest.AppendText(item.Key + " " + item.Value + " hou ");
                            //}

                            //txtTest.AppendText("nodes_hd\n");

                            //for (int k = 0; k < nodes_HD.Length;k++ )
                            //{
                            //    txtTest.AppendText("nodes_hd" + nodes_HD[k] + " mao ");
                            //}
                            weight_CR_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["CR"]).ToString()));
                            weight_ET_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["ET"]).ToString()));
                            weight_YE_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["YE"]).ToString()));
                            weight_XL_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["XL"]).ToString()));
                            weight_YJ_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["YJ"]).ToString()));
                            weight_HW_HD.Add(drOperate["HD"].ToString().Trim(), Convert.ToInt32((drOperate["HW"]).ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtTest.AppendText("HD ERROR: " + ex.Message + "\n");
            }
            if (dtNew.Columns["DATE_HBH_DW_HX_PBM"] != null)
                dtNew.Columns.Remove("DATE_HBH_DW_HX_PBM");
        }


        /// <summary>
        /// 通过流向数据计算航段数据的函数，在多行，最后一行时调用
        /// </summary>
        /// <param name="nodes">地名字符串数组</param>
        /// <param name="weight">地点对和流向数据的字典</param>
        /// <returns>地点对和航段数据的字典</returns>
        private Dictionary<String, int> doWithHD(string[] nodes, Dictionary<String, int> weight)
        {
            int nodesLen = nodes.Length;
            int sum;

            Dictionary<String, int> result = new Dictionary<String, int>();

            try
            {

                for (int i = 0; i < nodesLen; i++)
                {
                    for (int j = i + 1; j < nodesLen; j++)
                    {
                        result.Add(nodes[i] + "-" + nodes[j], 0);
                    }
                }
                for (int i = 0; i < nodesLen - 1; i++)
                {
                    sum = 0;
                    for (int j = 0; j < i + 1; j++)
                    {
                        for (int k = i + 1; k < nodesLen; k++)
                        {
                            sum += weight[nodes[j] + "-" + nodes[k]];
                        }

                        result[nodes[i] + "-" + nodes[i + 1]] = sum;
                    }
                }
            }
            catch (Exception ex)
            {
                txtTest.AppendText("doWithHD: " + ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 输出DBF文件
        /// </summary>
        private void outputDBF()
        {
            //dtNew.TableName = "New";
            txtTest.AppendText("Writing to: " + dtNew.TableName + ".dbf ...");

            //连接字符串
            string sConn =
                "Provider=Microsoft.Jet.OLEDB.4.0; " +
                "Data Source=" + System.IO.Directory.GetCurrentDirectory() + "; " +
                "Extended Properties=dBASE IV;";
            OleDbConnection conn = new OleDbConnection(sConn);
            conn.Open();    

            int columnCount = dtNew.Columns.Count;
            try
            {
                //如果存在同名文件则先删除
                if (File.Exists(dtNew.TableName + "_U.DBF"))
                {
                    txtTest.AppendText("Delete file: " + dtNew.TableName + "_U.DBF ...");
                    File.Delete(dtNew.TableName + "_U.DBF");
                }

                OleDbCommand cmd;

                //建立新表
                StringBuilder sbCreate = new StringBuilder();

                #region 通用生成建表语句
                //sbCreate.Append("CREATE TABLE " + dtNew.TableName + ".dbf (");
                //for (int i = 0; i < dtNew.Columns.Count; i++)
                //{
                //    sbCreate.Append(dtNew.Columns[i].ColumnName);
                //    sbCreate.Append(" char(20)");
                //    if (i != dtNew.Columns.Count - 1)
                //    {
                //        sbCreate.Append(", ");
                //    }
                //    else
                //    {
                //        sbCreate.Append(')');
                //    }
                //}
                #endregion

                //插入各行
                StringBuilder sbInsert = new StringBuilder();
                #region 通用生成插入sql语句
                //foreach (DataRow dr in dtNew.Rows)
                //{
                //    sbInsert.Clear();
                //    sbInsert.Append("INSERT INTO " + dtNew.TableName + ".dbf(");
                //    for (int i = 0; i < columnCount; i++)
                //    {
                //        sbInsert.Append(dtNew.Columns[i].ColumnName);
                //        if (i != columnCount - 1)
                //        {
                //            sbInsert.Append(", ");
                //        }
                //    }
                //    sbInsert.Append(") VALUES (");
                //    for (int i = 0; i < columnCount; i++)
                //    {
                //        sbInsert.Append("'" + @dr[i].ToString() + "'");
                //        if (i != columnCount - 1)
                //        {
                //            sbInsert.Append(", ");
                //        }
                //    }
                //    sbInsert.Append(')');

                //    //txtTest.AppendText(sbInsert + "\n");

                //    cmd = new OleDbCommand(sbInsert.ToString(), conn);
                //    cmd.ExecuteNonQuery();
                //}
                #endregion
                if (methodName == "YY")
                {
                    #region 写死的建表语句
                    sbCreate.Append("CREATE TABLE  " + dtNew.TableName + "_U.dbf("
                        + "DATE1 char(8),"
                        + "HBH char(8),"
                        + "CODE_SHARE char(30),"
                        + "DW char(2),"
                        + "FXD char(5),"
                        + "JH char(8),"
                        + "JX char(3),"
                        + "ZDYZ int,"
                        + "ZDZW int,"
                        + "HBXZ char(3),"
                        + "HXFL char(3),"
                        + "BC char(1),"
                        + "HX char(39),"
                        + "HD char(7),"
                        + "HDFL char(3),"
                        + "BC_HD char(1),"
                        + "HDJL char(5),"
                        + "R_HDJL char(5),"
                        + "DMSJ char(5),"
                        + "KZSJ char(5),"
                        + "APU char(5),"
                        + "APUSJ char(5),"
                        + "YCY int,"
                        + "XJY int,"
                        + "LCY int,"
                        + "KGYZ int,"
                        + "KGZW int,"
                        + "CR int,"
                        + "ET int,"
                        + "YE int,"
                        + "TD int,"
                        + "GW int,"
                        + "XL int,"
                        + "YJ int,"
                        + "HW int,"
                        + "CR_HD int,"
                        + "ET_HD int,"
                        + "YE_HD int,"
                        + "XL_HD int,"
                        + "YJ_HD int,"
                        + "HW_HD int,"
                        + "PBM char(18),"
                        + "CLDSK char(12),"
                        + "HXSK char(12),"
                        + "QFSK char(12),"
                        + "JLSK char(12),"
                        + "SLDSK char(12),"
                        + "GMT char(1),"
                        + "BJ char(1),"
                        + "BJD char(16)"
                        + ") "
                    );
                    #endregion

                    txtTest.AppendText(sbCreate.ToString());
                    cmd = new OleDbCommand(sbCreate.ToString(), conn);
                    cmd.ExecuteNonQuery();
                    foreach (DataRow dr in dtNew.Rows)
                    {
                        sbInsert.Clear();
                        #region 写死的插入语句
                        sbInsert.Append("INSERT INTO " + dtNew.TableName + "_U.dbf("
                                + "DATE1, HBH, CODE_SHARE, DW, FXD, JH, JX, ZDYZ, ZDZW, HBXZ, "
                                + "HXFL, BC, HX, HD, HDFL, BC_HD, HDJL, R_HDJL, DMSJ, KZSJ, "
                                + "APU, APUSJ, YCY, XJY, LCY, KGYZ, KGZW, CR, ET, YE, "
                                + "TD, GW, XL, YJ, HW, CR_HD, ET_HD, YE_HD, XL_HD, YJ_HD, "
                                + "HW_HD, PBM, CLDSK, HXSK, QFSK, JLSK, SLDSK, GMT, BJ, BJD"
                                + ") VALUES ("
                                + "\"" + dr["DATE1"].ToString().Trim() + "\"" + "," + "\"" + dr["HBH"].ToString().Trim() + "\"" + "," + "\"" + dr["CODE_SHARE"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["DW"].ToString().Trim() + "\"" + "," + "\"" + dr["FXD"].ToString().Trim() + "\"" + "," + "\"" + dr["JH"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["JX"].ToString().Trim() + "\"" + "," + dr["ZDYZ"].ToString().Trim() + "," + dr["ZDZW"].ToString().Trim() + ","
                                + "\"" + dr["HBXZ"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HXFL"].ToString().Trim() + "\"" + "," + "\"" + dr["BC"].ToString().Trim() + "\"" + "," + "\"" + dr["HX"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HD"].ToString().Trim() + "\"" + "," + "\"" + dr["HDFL"].ToString().Trim() + "\"" + "," + "\"" + dr["BC_HD"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HDJL"].ToString().Trim() + "\"" + "," + "\"" + dr["R_HDJL"].ToString().Trim() + "\"" + "," + "\"" + dr["DMSJ"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["KZSJ"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["APU"].ToString().Trim() + "\"" + "," + "\"" + dr["APUSJ"].ToString().Trim() + "\"" + "," + dr["YCY"].ToString().Trim() + ","
                                + dr["XJY"].ToString().Trim() + "," + dr["LCY"].ToString().Trim() + "," + dr["KGYZ"].ToString().Trim() + ","
                                + dr["KGZW"].ToString().Trim() + "," + dr["CR"].ToString().Trim() + "," + dr["ET"].ToString().Trim() + ","
                                + dr["YE"].ToString().Trim() + ","
                                + dr["TD"].ToString().Trim() + "," + dr["GW"].ToString().Trim() + "," + dr["XL"].ToString().Trim() + ","
                                + dr["YJ"].ToString().Trim() + "," + dr["HW"].ToString().Trim() + "," + dr["CR_HD"].ToString().Trim() + ","
                                + dr["ET_HD"].ToString().Trim() + "," + dr["YE_HD"].ToString().Trim() + "," + dr["XL_HD"].ToString().Trim() + ","
                                + dr["YJ_HD"].ToString().Trim() + ","
                                + dr["HW_HD"].ToString().Trim() + "," + "\"" + dr["PBM"].ToString().Trim() + "\"" + "," + "\"" + dr["CLDSK"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HXSK"].ToString().Trim() + "\"" + "," + "\"" + dr["QFSK"].ToString().Trim() + "\"" + "," + "\"" + dr["JLSK"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["SLDSK"].ToString().Trim() + "\"" + "," + "\"" + dr["GMT"].ToString().Trim() + "\"" + "," + "\"" + dr["BJ"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["BJD"].ToString().Trim() + "\""
                                + ")");
                        #endregion
                        //txtTest.AppendText(sbInsert + "\n");

                        cmd = new OleDbCommand(sbInsert.ToString(), conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                else if (methodName == "YT")
                {
                    #region 写死的建表语句
                    sbCreate.Append("CREATE TABLE  " + dtNew.TableName + "_U.dbf ("
                        + "DATE1 char(8),"
                        + "DW char(2),"
                        + "FXD char(5),"
                        + "HBH char(8), "
                        + "JH char(8),"
                        + "JX char(3),"
                        + "ZDYZ int,"
                        + "ZDZW int,"
                        + "HBXZ char(3),"
                        + "DQDH char(2),"
                        + "DQMC char(10),"
                        + "HX char(39),"
                        + "HD char(7),"
                        + "HDJL char(5),"
                        + "DMSJ char(5),"
                        + "KZSJ char(5),"
                        + "CLDSK char(12),"
                        + "HXSK char(12), "
                        + "QFSK char(12), "
                        + "JLSK char(12), "
                        + "SLDSK char(12),"
                        + "APU char(5),"
                        + "APUSJ char(5),"
                        + "YCY int,"
                        + "XJY int,"
                        + "LCY int,"
                        + "ZYMJ int,"
                        + "BC int,"
                        + "TYHK char(1),"
                        + "PBM char(18)"
                        + ") "
                    );
                    #endregion
                    txtTest.AppendText(sbCreate + "\n");
                    cmd = new OleDbCommand(sbCreate.ToString(), conn);
                    cmd.ExecuteNonQuery();
                    foreach (DataRow dr in dtNew.Rows)
                    {
                        sbInsert.Clear();
                        #region 写死的插入语句
                        sbInsert.Append("INSERT INTO " + dtNew.TableName + "_U.dbf("
                                + "DATE1, DW, FXD, HBH, JH, JX, ZDYZ, ZDZW, HBXZ, DQDH, "
                                + "DQMC, HX, HD, HDJL, DMSJ, KZSJ, CLDSK, HXSK, QFSK, JLSK, "
                                + "SLDSK, APU, APUSJ, YCY, XJY, LCY, ZYMJ, BC, TYHK, PBM"
                                + ") VALUES ("
                                + "\"" + dr["DATE1"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["DW"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["FXD"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HBH"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["JH"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["JX"].ToString().Trim() + "\"" + ","
                                + dr["ZDYZ"].ToString().Trim() + ","
                                + dr["ZDZW"].ToString().Trim() + ","
                                + "\"" + dr["HBXZ"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["DQDH"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["DQMC"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HX"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HD"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HDJL"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["DMSJ"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["KZSJ"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["CLDSK"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["HXSK"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["QFSK"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["JLSK"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["SLDSK"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["APU"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["APUSJ"].ToString().Trim() + "\"" + ","
                                + dr["YCY"].ToString().Trim() + ","
                                + dr["XJY"].ToString().Trim() + ","
                                + dr["LCY"].ToString().Trim() + ","
                                + dr["ZYMJ"].ToString().Trim() + ","
                                + dr["BC"].ToString().Trim() + ","
                                + "\"" + dr["TYHK"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["PBM"].ToString().Trim() + "\""
                                + ")");
                        #endregion
                        txtTest.AppendText(sbInsert + "\n");

                        cmd = new OleDbCommand(sbInsert.ToString(), conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    #region 写死的建表语句
                    sbCreate.Append("CREATE TABLE  " + dtNew.TableName + "_U.dbf ("
                        + "DATE1 char(8),"
                        + "DW char(2),"
                        + "FXD char(5),"
                        + "JH char(8),"
                        + "JX char(3),"
                        + "QMJS int, "
                        + "ZCTS int, "
                        + "KYTS int, "
                        + "PBM char(18),"
                        + "BZ char(140)"
                        + ")"
                    );
                    #endregion
                    cmd = new OleDbCommand(sbCreate.ToString(), conn);
                    cmd.ExecuteNonQuery();
                    foreach (DataRow dr in dtNew.Rows)
                    {
                        sbInsert.Clear();
                        #region 写死的插入语句
                        sbInsert.Append("INSERT INTO " + dtNew.TableName + "_U.dbf("
                                + "DATE1, DW, FXD, HBH, JH, JX, ZDYZ, ZDZW, HBXZ, DQDH, "
                                + "DQMC, HX, HD, HDJL, DMSJ, KZSJ, CLDSK, HXSK, QFSK, JLSK, "
                                + "SLDSK, APU, APUSJ, YCY, XJY, LCY, ZYMJ, BC, TYHK, PBM"
                                + ") VALUES ("
                                + "\"" + dr["DATE1"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["DW"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["FXD"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["JH"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["JX"].ToString().Trim() + "\"" + ","
                                + dr["QMJS"].ToString().Trim() + ","
                                + dr["ZCTS"].ToString().Trim() + ","
                                + dr["KYTS"].ToString().Trim() + ","
                                + "\"" + dr["PBM"].ToString().Trim() + "\"" + ","
                                + "\"" + dr["BZ"].ToString().Trim() + "\"" + ","
                                + ")");
                        #endregion
                        //txtTest.AppendText(sbInsert + "\n");

                        cmd = new OleDbCommand(sbInsert.ToString(), conn);
                        cmd.ExecuteNonQuery();
                    }
                }

                #region 修改字段名date1为date
                //sbInsert.Clear();
                //txtTest.AppendText("alter table " + dtNew.TableName + "_U.dbf rename column date1 to date\r\n");
                //sbInsert.Append("alter table " + dtNew.TableName + ".dbf rename column date1 to date");
                //cmd = new OleDbCommand(sbInsert.ToString(), conn);
                //cmd.ExecuteNonQuery();
                #endregion

            }
            catch (Exception ex)
            {
                txtTest.AppendText(ex.Message);
                conn.Close();
            }
            conn.Close();
        }

        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            endTime = DateTime.Now;
            subTime = endTime - startTime;
            this.lab_time_show.Text = subTime.Minutes.ToString().PadLeft(2, '0') + ":" + subTime.Seconds.ToString().PadLeft(2, '0') + ":" + subTime.Milliseconds.ToString().PadLeft(3, '0');
        }

        /// <summary>
        /// 停止按钮点击触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stop_Click(object sender, EventArgs e)
        {
            thread.Abort();
            btn_stop.Enabled = false;
            if (File.Exists(dtNew.TableName + ".dbf"))
            {
                txtTest.AppendText("Delete file: " + dtNew.TableName + ".dbf ...");
                File.Delete(dtNew.TableName + ".dbf");
            }
            btn_inputFile.Enabled = true;
            btn_convertToDbf.Enabled = true;
            btn_convertToXlsx.Enabled = true;
            timer1.Enabled = false;
            radioBtnYF.Enabled = true;
            radioBtnYT.Enabled = true;
            radioBtnYY.Enabled = true;
        }











        private void btn_convertToXlsx_Click(object sender, EventArgs e)
        {
            txtTest.Text = "";
            timer1.Enabled = true;
            btn_stop.Enabled = true;

            thread = new Thread(initXlsx);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// XLSX运行函数
        /// </summary>
        private void initXlsx()
        {
            startTime = DateTime.Now;

            btn_convertToXlsx.Enabled = false;
            btn_inputFile.Enabled = false;
            btn_convertToDbf.Enabled = false;
            radioBtnYY.Enabled = false;
            radioBtnYT.Enabled = false;
            radioBtnYF.Enabled = false;

            //写入DBF
            inputXlsx();

            txtTest.AppendText(dtOld.Rows.Count.ToString() + " " + dtOld.Columns.Count.ToString() + "\n");
            if (dtOld != null && dtOld.Rows.Count > 0)
            {
                dtNew = dtOld.Copy();
                dtNew.TableName = fileName;

                if (methodName == "YY")
                {
                    init();
                    doWithInitHD();
                    doWithDelete();
                    doWithAdd();
                    doWithCODE_SHARE();
                    doWithHD();
                    doWithOrder();
                    outputXlsx();
                }
                else if (methodName == "YT")
                {
                    doWithDelete();
                    doWithAdd();
                    doWithOrder();
                    outputXlsx();
                }
                else
                {
                    init();
                    outputXlsx();
                }
                #region YY
                //txtTest.AppendText("InitHD\n");
                //doWithInitHD();
                //txtTest.AppendText("Delete\n");
                //doWithDelete();
                //txtTest.AppendText("Add\n");
                //doWithAdd();
                ////doWithCODE_SHARE();
                //txtTest.AppendText("HD\n");
                //doWithHD();
                //outputXlsx();
                #endregion
            }

            btn_convertToXlsx.Enabled = true;
            btn_inputFile.Enabled = true;
            btn_convertToDbf.Enabled = true;
            btn_stop.Enabled = false;
            radioBtnYY.Enabled = true;
            radioBtnYT.Enabled = true;
            radioBtnYF.Enabled = true;
            radioBtnYF.Checked = false;
            radioBtnYT.Checked = false;
            radioBtnYY.Checked = false;

            timer1.Enabled = false;
            txtTest.AppendText("over");
            MessageBox.Show("数据转换完毕", "提示");
        }


        /// <summary>
        /// 调整字段顺序函数
        /// </summary>
        private void doWithOrder()
        {
            if (methodName == "YY")
            {
                dtNew.Columns["CODE_SHARE"].SetOrdinal(2);
                dtNew.Columns["R_HDJL"].SetOrdinal(17);
                dtNew.Columns["APUSJ"].SetOrdinal(21);
                dtNew.Columns["CR_HD"].SetOrdinal(35);
                dtNew.Columns["ET_HD"].SetOrdinal(36);
                dtNew.Columns["YE_HD"].SetOrdinal(37);
                dtNew.Columns["XL_HD"].SetOrdinal(38);
                dtNew.Columns["YJ_HD"].SetOrdinal(39);
                dtNew.Columns["HW_HD"].SetOrdinal(40);
                dtNew.Columns["HXSK"].SetOrdinal(43);
            }
            else if (methodName == "YT")
            {
                dtNew.Columns["HBH"].SetOrdinal(3);
                dtNew.Columns["CLDSK"].SetOrdinal(16);
                dtNew.Columns["HXSK"].SetOrdinal(17);
                dtNew.Columns["QFSK"].SetOrdinal(18);
                dtNew.Columns["JLSK"].SetOrdinal(19);
                dtNew.Columns["SLDSK"].SetOrdinal(20);
                dtNew.Columns["APUSJ"].SetOrdinal(22);
            }
        }

        /// <summary>
        /// 输入xlsx文件到datatable中
        /// </summary>
        private void inputXlsx()
        {
            string strConn2;
            string filePath = inPath;
            FileInfo fileInfo = new FileInfo(filePath);
            string directory = fileInfo.DirectoryName;

            try
            {
                strConn2 = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;';";
                string strConnection = string.Format(strConn2, inPath);

                OleDbConnection conn = new OleDbConnection(strConnection);
                conn.Open();
                String tableName = null;
                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                tableName = dt.Rows[0][2].ToString().Trim();
                txtTest.AppendText(tableName);
                OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + tableName + "]", strConnection);
                dtOld.TableName = "Old";
                oada.Fill(dtOld);//获得datatable
                conn.Close();
            }
            catch (Exception ex)
            {
                txtTest.AppendText(ex.Message + "\n");
            }
        }



        /// <summary>
        /// 从datatable输出到.xlsx文件中
        /// </summary>
        private void outputXlsx()
        {
            FileInfo newFile = new FileInfo(dtNew.TableName + ".xlsx");
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(dtNew.TableName + ".xlsx");
            }
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dtNew.TableName);//创建worksheet

                for (int m = 0; m < dtNew.Columns.Count; m++)
                {
                    worksheet.Cells[1, m + 1].Value = dtNew.Columns[m];
                }
                int i = 1;
                foreach (DataRow dr in dtNew.Rows)
                {
                    for (int j = 0; j < dtNew.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 1, j + 1].Value = dr[j];
                    }
                    i++;
                }
                package.Save();//保存excel
            }
        }
        //private void outputXlsx()
        //{
        //    //如果存在同名文件则先删除
        //    if (File.Exists(dtNew.TableName + ".xlsx"))
        //    {
        //        txtTest.AppendText("Delete file: " + dtNew.TableName + ".xlsx ...");
        //        File.Delete(dtNew.TableName + ".xlsx");
        //    }

        //    XSSFWorkbook xssfworkbook = new XSSFWorkbook();
        //    XSSFSheet sheet1 = (XSSFSheet)xssfworkbook.CreateSheet("Sheet1");
        //    XSSFRow row;
        //    XSSFCell cell;
        //    DataRow opDaterow;
        //    //FileStream file = new FileStream(dtNew.TableName + ".xlsx", FileMode.Create);

        //    txtTest.AppendText("Begin");
        //    //写入字段名
        //    row = (XSSFRow)sheet1.CreateRow(0);
        //    for (int i = 0; i < dtNew.Columns.Count; i++)
        //    {
        //        cell = (XSSFCell)row.CreateCell(i);
        //        cell.SetCellValue(dtNew.Columns[i].ColumnName);
        //    }
        //    txtTest.AppendText("Mid");
        //    for (int i = 0; i < dtNew.Rows.Count; i++)
        //    {
        //        opDaterow = dtNew.Rows[i];

        //        row = (XSSFRow)sheet1.CreateRow(i + 1);
        //        for (int j = 0; j < dtNew.Columns.Count; j++)
        //        {
        //            cell = (XSSFCell)row.CreateCell(j);
        //            cell.SetCellValue(opDaterow[j].ToString());

        //        }
        //    }
        //    txtTest.AppendText("Finish");

        //    try
        //    {
        //        FileStream file = new FileStream(dtNew.TableName + ".xlsx", FileMode.Create);
        //        txtTest.AppendText("111");
        //        xssfworkbook.Write(file);
        //        txtTest.AppendText("222");
        //        file.Close();
        //        txtTest.AppendText("333");
        //    }
        //    catch (Exception ex)
        //    {
        //        txtTest.AppendText(ex.Message);
        //    }
        //}

        private void radioBtnYY_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnYY.Checked)
            {
                methodName = "YY";
            }
            if (txt_showPath.Text != "")
            {
                btn_convertToDbf.Enabled = true;
                btn_convertToXlsx.Enabled = true;
            }
        }

        private void radioBtnYT_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnYT.Checked)
            {
                methodName = "YT";
            }
            if (txt_showPath.Text != "")
            {
                btn_convertToDbf.Enabled = true;
                btn_convertToXlsx.Enabled = true;
            }
        }

        private void radioBtnYF_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnYF.Checked)
            {
                methodName = "YF";
            }
            if (txt_showPath.Text != "")
            {
                btn_convertToDbf.Enabled = true;
                btn_convertToXlsx.Enabled = true;
            }
        }

        private void btn_progressBar_Click(object sender, EventArgs e)
        {
            proBarThread = new Thread(SleepT);
            proBarThread.Start();
        }
        private void SleepT()
        {
            for (int i = 0; i <= 500; i++)
            {
                System.Threading.Thread.Sleep(10);
                SetTextMesssage(100 * i / 500, i.ToString() + "\r\n");
            }
        }

        private void SetTextMesssage(int ipos, string vinfo)
        {
            if (this.InvokeRequired)
            {
                SetPos setpos = new SetPos(SetTextMesssage);
                this.Invoke(setpos, new object[] { ipos, vinfo });
            }
            else
            {
                this.lab_test.Text = ipos.ToString() + "/100";
                this.progressBar1.Value = Convert.ToInt32(ipos);
                this.txtTest.AppendText(vinfo);
            }
        }
    }
}
