using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CEMerge
{
    /**
 * 中英合并工具类，用于辅助中英文本的合并
 */
    public class CemTool
    {
        private readonly Regex _regex = new Regex("\\(.\\)");

        /**
     * 基础合并
     */
        public string Merge(string translation, string originalText)
        {
            return translation + "(" + originalText + ")";
        }

        /**
     * 优化合并：
     * 文本相同不合并
     * 文本中含有换行则合并后中英文本之间也有换行
     */
        public string OptimizedMerge(string translation, string originalText)
        {
            if (translation.Equals(originalText))
            {
                return translation;
            }

            if (translation.Contains("\n"))
            {
                if (originalText.EndsWith("\n"))
                {
                    if (originalText.EndsWith("\r\n"))
                    {
                        return (translation + "(" + originalText + ")").Replace("\r\n)", ")");
                    }

                    return (translation + "(" + originalText + ")").Replace("\n)", ")");
                }

                return translation + "\n(" + originalText + ")";
            }

            if (translation.Contains("<br>"))
            {
                if (originalText.EndsWith("<br>"))
                {
                    return (translation + "(" + originalText + ")").Replace("<br>)", ")");
                }

                return translation + "<br>(" + originalText + ")";
            }

            return Merge(translation, originalText);
        }

        /**
     * 中英冒号和括号优化
     * 如 他说：(He said:)（XXX)
     * 将会替换为 他说(He said):（XXX)
     */
        public string ColonOptimize(string mergedText)
        {
            if (mergedText.Contains("：(") && mergedText.Contains(":)"))
            {
                mergedText = mergedText.Replace("：(", "(");
                mergedText = mergedText.Replace(":)", "):");
            }

            return mergedText;
        }

        /**
     * 中文标点优化
     */
        public string ChinesePunctuationToEnglish(String mergedText)
        {
            mergedText = mergedText.Replace("！", "!");
            mergedText = mergedText.Replace("—", "-");
            mergedText = mergedText.Replace("？", "?");
            mergedText = mergedText.Replace("…", "...");
            mergedText = mergedText.Replace("（", "(");
            mergedText = mergedText.Replace("）", ")");
            return mergedText;
        }

        /**
     * html标签优化，目前只有b和i
     */
        public string HtmlTagsOptimize(String mergedText)
        {
            mergedText = mergedText.Replace("{/b}({b}", "(");
            mergedText = mergedText.Replace("{/i}({i}", "(");
            mergedText = mergedText.Replace("{/b})", "){/b}");
            mergedText = mergedText.Replace("{/i})", "){/i}");
            mergedText = mergedText.Replace("</b>(<b>", "(");
            mergedText = mergedText.Replace("</i>(<i>", "(");
            mergedText = mergedText.Replace("</b>)", ")</b>");
            mergedText = mergedText.Replace("</i>)", ")</i>");
            mergedText = mergedText.Replace("<br>(<br>)", "<br>");
            mergedText = mergedText.Replace("<br><br>(", "<br>(");
            mergedText = mergedText.Replace("<br><br>(", "<br>(");
            while (mergedText.EndsWith("<br>)"))
            {
                mergedText = mergedText.Replace("<br>)", ")");
            }

            return mergedText;
        }

        /**
     * 双重括号优化
     */
        public string DoubleBracketOptimize(String mergedText)
        {
            string mergedTextNew = mergedText.Replace("((", "(");
            mergedTextNew = mergedTextNew.Replace("))", ")");
            if (!BracketMatched(mergedTextNew))
            {
                return mergedText;
            }

            return mergedTextNew;
        }


        private bool BracketMatched(String strs)
        {
            var s = new Stack<char>();
            foreach (var c in strs)
            {
                if (c == '(')
                    s.Push(')');
                else if (c == '[')
                    s.Push(']');
                else if (c == '{')
                    s.Push('}');
                else if (s.Count == 0 || c != s.Pop())
                    return false;
            }

            return s.Count == 0;
        }


        public bool ContainsModifierParameters(String text, String[] inputParameters)
        {
            bool contains = false;
            foreach (String modifierParameter in inputParameters)
            {
                if (text == null)
                {
                    return false;
                }

                if (text.Contains(modifierParameter))
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }

        /**
     * 处理带有参数的文本
     */
        public string DealWithStringsWithParameters(string translation, string originalText, string[] inputParameters,
            bool paramsHaveBoldTag)
        {
            int parametersCount = 0; //文本中包含的预设参数的数量
            var parameterMap = new Dictionary<int, string>(); //映射
            var parameters = new string[inputParameters.Length];
            var parametersIndex = new int[inputParameters.Length];
            foreach (string parameter in inputParameters)
            {
                //在当前英文文本中查找有哪些预设参数
                if (originalText.Contains(parameter))
                {
                    parametersIndex[parametersCount] = originalText.IndexOf(parameter);
                    parameterMap[parametersIndex[parametersCount]] = parameter;
                    parametersCount++;
                }
            }

            Array.Sort(parametersIndex, 0, parametersCount);
            for (int i = 0; i < parametersCount; i++)
            {
                parameters[i] = parameterMap[parametersIndex[i]];
            }

            //以上，将参数按在文本中出现的顺序排好，放入parameters数组中
            String[] splitTrans = new String[2 * parametersCount + 1];
            for (int i = 0; i < parametersCount; i++)
            {
                string sp = i == 0 ? originalText : splitTrans[2 * i];
                int preTextEnd = sp.IndexOf(parameters[i]);
                preTextEnd--;
                int nextTextStart = preTextEnd + parameters[i].Length;
                nextTextStart++;
                while (preTextEnd >= 0 && sp[preTextEnd] == ' ')
                {
                    preTextEnd--;
                }

                while (nextTextStart < sp.Length && sp[nextTextStart] == ' ')
                {
                    nextTextStart++;
                }

                splitTrans[i * 2] = sp.Substring(0, preTextEnd + 1);
                string boldTagedParameter = "<b>" + parameters[i] + "</b>";
                if (paramsHaveBoldTag && translation.Contains(boldTagedParameter))
                {
                    //如果有<b>标签则算入
                    splitTrans[i * 2 + 1] = boldTagedParameter;
                }
                else
                {
                    splitTrans[i * 2 + 1] = parameters[i];
                }

                splitTrans[i * 2 + 2] = sp.Substring(nextTextStart);
            }

            StringBuilder merge = new StringBuilder();
            int cStart = 0, cEnd, j;
            //以上，将英文文本按照参数切分成文本参数文本参数间隔的形式存入splitTrans
            for (j = 0; j < parametersCount; j++)
            {
                cEnd = translation.IndexOf(splitTrans[2 * j + 1]);
                if (cEnd < cStart)
                {
                    return "ChnAndEngGrammarOrderFail";
                }

                merge.Append(translation, cStart, cEnd - cStart).Append("(").Append(splitTrans[2 * j]).Append(")")
                    .Append(splitTrans[2 * j + 1]);
                cStart = cEnd + splitTrans[2 * j + 1].Length;
            }

            //以上，按顺序读中文文本，遇到参数，在之前添加英文翻译
            merge.Append(translation.Substring(cStart)).Append("(").Append(splitTrans[2 * j]).Append(")"); //文本尾部处理
            merge = new StringBuilder(merge.ToString().Replace("()", ""));
            return _regex.Replace(merge.ToString(), ""); //删除不必要的元素
        }
    }
}