using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CEMerge
{
    public class CemBaldursGate3
    {
        /*
         * larian框架游戏中英文本合并,适用于博得之门3
         * 尽量不要用于神界原罪2
         * 需要准备中英文本，可选修正文本
         * 相对于上一个版本的代码，此处读取xml采用了dom的方式
         * 并且开头结尾的contentList标签不需手动删除添加
         * 博得之门3的version属性也得以保留，且优化了代码，运行速度加快
         * 个人修正文件需和中英文本格式相同
         */

        public static void MergeTranslations(string englishPathName, string chinesePathName, string writePathName, string personalRevisePathName)
        {
            Console.WriteLine("Merging Localizations...");
            bool personalReviseExist = File.Exists(personalRevisePathName);
            var cem = new CemTool();
            string[] parameters = { "[1]", "[2]", "[3]", "[4]", "[5]", "[6]", "[7]" };
            try
            {
                var transMap = new Dictionary<string, string>(); //英文文本保存的map
                Dictionary<string, string> reviseMap = null; //个人修正文本保存的map
                var chineseDocument = new XmlDocument(); //读取后的中文文件
                chineseDocument.Load(chinesePathName);
                var englishDocument = new XmlDocument(); //读取后的英文文件
                englishDocument.Load(englishPathName);
                var chineseDocumentList = chineseDocument.SelectSingleNode("contentList"); //读取后的中文内容list
                var englishDocumentList = englishDocument.SelectSingleNode("contentList"); //读取后的英文内容list
                foreach (XmlElement englishElement in englishDocumentList)
                {
                    //将英文uid和文本存入map便于查询
                    transMap[englishElement.GetAttribute("contentuid")] = englishElement.InnerText;
                }

                if (personalReviseExist)
                {
                    Console.WriteLine("Revise text exists.");
                    //存在个人修正文本则读取并存入修正的map
                    reviseMap = new Dictionary<string, string>();
                    XmlDocument reviseDocument = new XmlDocument();
                    reviseDocument.Load(personalRevisePathName);
                    var reviseDocumentList = reviseDocument.SelectSingleNode("contentList");
                    foreach (XmlElement reviseElement in reviseDocumentList)
                    {
                        reviseMap[reviseElement.GetAttribute("contentuid")] = reviseElement.InnerText;
                    }
                }

                var sw = new StreamWriter(writePathName);
                string englishText;
                string chineseText;
                string mergedText;
                string mergedCoreText;
                sw.WriteLine("<contentList>"); //开头写入一个<contentList>
                foreach (XmlElement chineseElement in chineseDocumentList)
                {
                    //将中文文本取出与英文文本合并再存入文件
                    var contentuid = chineseElement.GetAttribute("contentuid"); //取出当前行的contentuid的属性值
                    transMap.TryGetValue(contentuid, out englishText); //从英文map中根据uid取出对应的英文文本
                    mergedText = "	<content contentuid=\"" + contentuid + "\""; //合并的文本
                    mergedText += " version=\"" + chineseElement.GetAttribute("version") + "\"";
                    mergedText += ">";
                    if (personalReviseExist && reviseMap.TryGetValue(contentuid, out mergedCoreText))
                    {
                        //如果有个人修正文本则进行修正
                        mergedCoreText = HtmlCharChange(mergedCoreText);
                        mergedText += mergedCoreText;
                    }
                    else
                    {
                        //无则将原来的中英文本组合
                        bool containsModifierParameters = cem.ContainsModifierParameters(englishText, parameters);
                        chineseText = chineseElement.InnerText;
                        if (containsModifierParameters)
                        {
                            mergedCoreText =
                                cem.DealWithStringsWithParameters(chineseText, englishText, parameters, false);
                            mergedCoreText = cem.ChinesePunctuationToEnglish(mergedCoreText);
                            if (mergedCoreText.Equals("ChnAndEngGrammarOrderFail"))
                            {
                                chineseText = cem.ChinesePunctuationToEnglish(chineseText);
                                mergedCoreText = cem.OptimizedMerge(chineseText, englishText);
                                // Console.WriteLine(contentuid);
                            }
                        }
                        else
                        {
                            if (englishText == null)
                            {
                                englishText = chineseText;
                            }

                            chineseText = cem.ChinesePunctuationToEnglish(chineseText);
                            mergedCoreText = cem.OptimizedMerge(chineseText, englishText);
                        }

                        mergedCoreText = cem.HtmlTagsOptimize(mergedCoreText);
                        mergedCoreText = cem.ChinesePunctuationToEnglish(mergedCoreText);
                        mergedCoreText = HtmlCharChange(mergedCoreText);
                        mergedText += mergedCoreText;
                        mergedText = cem.ColonOptimize(mergedText);
                        mergedText = cem.DoubleBracketOptimize(mergedText);
                        mergedText = cem.HtmlTagsOptimize(mergedText);
                    }

                    mergedText += "</content>";
//                System.out.println(mergedText); //此处解除注释可以输出合并的文本检查
                    sw.WriteLine(mergedText); //写入进xml
                }

                sw.WriteLine("</contentList>"); //最后加上</contentList>
                sw.Close();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
            Console.WriteLine("Localizations merged successfully.\n");
        }

        public static String HtmlCharChange(String text)
        {
            text = text.Replace("&", "&amp;");
            text = text.Replace("<", "&lt;");
            text = text.Replace(">", "&gt;");
            return text;
        }
    }
}