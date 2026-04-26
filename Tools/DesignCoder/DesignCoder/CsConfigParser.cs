using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DesignCoder
{
    public class FieldDef
    {
        public string Name;
        public string Type;
        public string Comment;
    }

    public class ConfigData
    {
        public string ClassName;
        public string Namespace;
        public List<FieldDef> Fields = new List<FieldDef>();
        public List<Dictionary<string, string>> Rows = new List<Dictionary<string, string>>();
        public string UsingSection;
        public string PreLoadCode;
        public string PostLoadCode;

        public static ConfigData Parse(string source)
        {
            var data = new ConfigData();

            var nsMatch = Regex.Match(source, @"namespace\s+(\w+)");
            if (nsMatch.Success)
                data.Namespace = nsMatch.Groups[1].Value;

            var classMatch = Regex.Match(source, @"public\s+class\s+(\w+)");
            if (classMatch.Success)
                data.ClassName = classMatch.Groups[1].Value;

            ParseFields(source, data);
            ParseLoadMethod(source, data);
            SplitCodeAroundLoad(source, data);

            return data;
        }

        private static void ParseFields(string source, ConfigData data)
        {
            string pattern = @"///\s*<summary>\s*\r?\n\s*///\s*(.*?)\s*\r?\n\s*///\s*</summary>\s*\r?\n\s*public\s+(\w+(?:\[\])?)\s+(\w+)\s*;";
            var matches = Regex.Matches(source, pattern);
            foreach (Match m in matches)
            {
                var fd = new FieldDef();
                fd.Comment = m.Groups[1].Value.Trim();
                fd.Type = m.Groups[2].Value.Trim();
                fd.Name = m.Groups[3].Value.Trim();
                data.Fields.Add(fd);
            }
        }

        private static void ParseLoadMethod(string source, ConfigData data)
        {
            int loadStart = source.IndexOf("public static void Load()");
            if (loadStart < 0) return;

            int braceStart = source.IndexOf('{', loadStart);
            if (braceStart < 0) return;

            int depth = 0;
            int loadEnd = -1;
            for (int i = braceStart; i < source.Length; i++)
            {
                if (source[i] == '{') depth++;
                else if (source[i] == '}')
                {
                    depth--;
                    if (depth == 0) { loadEnd = i; break; }
                }
            }
            if (loadEnd < 0) return;

            string loadBody = source.Substring(braceStart + 1, loadEnd - braceStart - 1);

            string entryPattern = @"config\s*\[\s*(\d+)\s*\]\s*=\s*new\s+" + Regex.Escape(data.ClassName) + @"\s*\(";
            var entryMatches = Regex.Matches(loadBody, entryPattern);

            foreach (Match em in entryMatches)
            {
                int argsStart = em.Index + em.Length;
                var args = ParseConstructorArgs(loadBody, argsStart);
                var row = new Dictionary<string, string>();
                for (int i = 0; i < data.Fields.Count && i < args.Count; i++)
                {
                    string displayVal = RawToDisplay(args[i], data.Fields[i].Type);
                    row[data.Fields[i].Name] = displayVal;
                }
                data.Rows.Add(row);
            }
        }

        private static List<string> ParseConstructorArgs(string source, int startIndex)
        {
            var args = new List<string>();
            int i = startIndex;
            int depth = 1;

            while (i < source.Length && depth > 0)
            {
                while (i < source.Length && char.IsWhiteSpace(source[i])) i++;
                if (i >= source.Length) break;

                if (source[i] == ')') { depth--; if (depth == 0) break; i++; continue; }
                if (source[i] == ',') { i++; continue; }

                string arg = ExtractOneArg(source, ref i);
                args.Add(arg);

                while (i < source.Length && char.IsWhiteSpace(source[i])) i++;
                if (i < source.Length && source[i] == ',') i++;
                else if (i < source.Length && source[i] == ')') { depth--; break; }
            }

            return args;
        }

        private static string ExtractOneArg(string source, ref int i)
        {
            while (i < source.Length && char.IsWhiteSpace(source[i])) i++;
            if (i >= source.Length) return "";

            if (source[i] == '"')
                return ExtractStringArg(source, ref i);

            if (i + 3 < source.Length && source.Substring(i, 4) == "new ")
                return ExtractNewArg(source, ref i);

            if (i + 3 < source.Length && source.Substring(i, 4) == "null")
            { i += 4; return "null"; }

            if (i + 3 < source.Length && source.Substring(i, 4) == "true")
            { i += 4; return "true"; }

            if (i + 4 < source.Length && source.Substring(i, 5) == "false")
            { i += 5; return "false"; }

            var sb = new StringBuilder();
            while (i < source.Length && source[i] != ',' && source[i] != ')')
            {
                sb.Append(source[i]);
                i++;
            }
            return sb.ToString().Trim();
        }

        private static string ExtractStringArg(string source, ref int i)
        {
            var sb = new StringBuilder();
            sb.Append(source[i]); i++;
            while (i < source.Length)
            {
                if (source[i] == '\\')
                {
                    sb.Append(source[i]); i++;
                    if (i < source.Length) { sb.Append(source[i]); i++; }
                    continue;
                }
                sb.Append(source[i]);
                if (source[i] == '"') { i++; break; }
                i++;
            }
            return sb.ToString();
        }

        private static string ExtractNewArg(string source, ref int i)
        {
            int d = 0;
            var sb = new StringBuilder();
            while (i < source.Length)
            {
                if (source[i] == '{' || source[i] == '(' || source[i] == '[') d++;
                else if (source[i] == '}' || source[i] == ')' || source[i] == ']') d--;

                sb.Append(source[i]);
                i++;

                if (d == 0)
                {
                    string trimmed = sb.ToString().TrimEnd();
                    if (trimmed.EndsWith("}") || trimmed.EndsWith(")"))
                    {
                        while (i < source.Length && (source[i] == ' ' || source[i] == '\t')) i++;
                        if (i >= source.Length || source[i] == ',' || source[i] == ')')
                            break;
                    }
                }
            }
            return sb.ToString().Trim();
        }

        private static string RawToDisplay(string raw, string type)
        {
            raw = raw.Trim();
            if (raw == "null") return "";

            if (type == "string")
            {
                if (raw.Length >= 2 && raw.StartsWith("\"") && raw.EndsWith("\""))
                    return raw.Substring(1, raw.Length - 2).Replace("\\\"", "\"");
                return raw;
            }

            if (type == "float")
            {
                string v = raw;
                if (v.EndsWith("f") || v.EndsWith("F")) v = v.Substring(0, v.Length - 1);
                return v;
            }

            if (type == "string[]")
                return ParseArrayToDisplay(raw, true);

            if (type == "int[]")
                return ParseArrayToDisplay(raw, false);

            return raw;
        }

        private static string ParseArrayToDisplay(string raw, bool isString)
        {
            var match = Regex.Match(raw, @"new\s+\w+\s*\[\s*\]\s*\{(.*)\}", RegexOptions.Singleline);
            if (!match.Success) return "";

            string inner = match.Groups[1].Value.Trim();
            if (string.IsNullOrWhiteSpace(inner)) return "";

            var items = new List<string>();
            int idx = 0;
            while (idx < inner.Length)
            {
                while (idx < inner.Length && char.IsWhiteSpace(inner[idx])) idx++;
                if (idx >= inner.Length) break;

                if (inner[idx] == '"')
                {
                    idx++;
                    var sb = new StringBuilder();
                    while (idx < inner.Length)
                    {
                        if (inner[idx] == '\\') { idx++; if (idx < inner.Length) { sb.Append(inner[idx]); idx++; } continue; }
                        if (inner[idx] == '"') { idx++; break; }
                        sb.Append(inner[idx]); idx++;
                    }
                    items.Add(sb.ToString());
                }
                else
                {
                    var sb = new StringBuilder();
                    while (idx < inner.Length && inner[idx] != ',')
                    {
                        sb.Append(inner[idx]); idx++;
                    }
                    string val = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(val)) items.Add(val);
                }

                while (idx < inner.Length && char.IsWhiteSpace(inner[idx])) idx++;
                if (idx < inner.Length && inner[idx] == ',') idx++;
            }

            return string.Join(",", items.ToArray());
        }

        private static void SplitCodeAroundLoad(string source, ConfigData data)
        {
            int loadStart = source.IndexOf("public static void Load()");
            if (loadStart < 0)
            {
                data.PreLoadCode = source;
                data.PostLoadCode = "";
                return;
            }

            int braceStart = source.IndexOf('{', loadStart);
            int depth = 0;
            int loadEnd = -1;
            for (int i = braceStart; i < source.Length; i++)
            {
                if (source[i] == '{') depth++;
                else if (source[i] == '}')
                {
                    depth--;
                    if (depth == 0) { loadEnd = i; break; }
                }
            }

            data.PreLoadCode = source.Substring(0, loadStart);
            data.PostLoadCode = source.Substring(loadEnd + 1);
        }

        public string GenerateSource()
        {
            var sb = new StringBuilder();

            sb.Append(PreLoadCode);

            sb.Append("public static void Load()");
            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("config.Clear();");

            foreach (var row in Rows)
            {
                sb.Append("config[");
                string idVal = row.ContainsKey("Id") ? row["Id"] : "0";
                sb.Append(idVal);
                sb.Append("] = new ");
                sb.Append(ClassName);
                sb.Append("(");

                for (int i = 0; i < Fields.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    string displayVal = row.ContainsKey(Fields[i].Name) ? row[Fields[i].Name] : "";
                    sb.Append(DisplayToRaw(displayVal, Fields[i].Type));
                }

                sb.Append(");");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("}");

            sb.Append(PostLoadCode);

            return sb.ToString();
        }

        private string DisplayToRaw(string display, string type)
        {
            if (type == "int")
            {
                if (string.IsNullOrEmpty(display)) return "0";
                return display.Trim();
            }

            if (type == "float")
            {
                if (string.IsNullOrEmpty(display)) return "0f";
                string v = display.Trim();
                if (!v.EndsWith("f") && !v.EndsWith("F")) v = v + "f";
                return v;
            }

            if (type == "string")
            {
                if (display == null) return "\"\"";
                return "\"" + display.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            }

            if (type == "bool")
            {
                if (string.IsNullOrEmpty(display)) return "false";
                return display.Trim().ToLower();
            }

            if (type == "string[]")
            {
                if (string.IsNullOrEmpty(display)) return "null";
                var items = display.Split(',');
                var sb = new StringBuilder();
                sb.Append("new string[]{");
                for (int i = 0; i < items.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("\"");
                    sb.Append(items[i].Trim().Replace("\\", "\\\\").Replace("\"", "\\\""));
                    sb.Append("\"");
                }
                sb.Append("}");
                return sb.ToString();
            }

            if (type == "int[]")
            {
                if (string.IsNullOrEmpty(display)) return "null";
                var items = display.Split(',');
                var sb = new StringBuilder();
                sb.Append("new int[]{");
                for (int i = 0; i < items.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(items[i].Trim());
                }
                sb.Append("}");
                return sb.ToString();
            }

            return display;
        }
    }
}
