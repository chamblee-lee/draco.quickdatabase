using System;
using System.Text;

namespace Draco.DB.ORM.Mapping.AttrMapping
{
	/// <summary>
	/// ���ֶνṹ����
	/// </summary>
    [AttributeUsage(AttributeTargets.Property,Inherited=true,AllowMultiple = false)]
	sealed  public class PrimaryKeyMappingAttribute : Attribute 
	{
        /// <summary>
        /// �ֶ�ӳ��
        /// </summary>
        private PrimaryKeyMapping m_Mapping=new PrimaryKeyMapping();

        /// <summary>
        /// ���캯��
        /// </summary>
        public PrimaryKeyMappingAttribute()
        {
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="stuExpress"></param>
        public PrimaryKeyMappingAttribute(string stuExpress)
        {
            this.m_Mapping = ParseTo(stuExpress);
        }

        #region ӳ������
        /// <summary>
        /// ���ݿ��ֶ�����
        /// </summary>
        public string ColumnName
        {
            set { this.m_Mapping.ColumnName = value; }
            get { return this.m_Mapping.ColumnName; }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public string PropertyName
        {
            set { this.m_Mapping.PropertyName = value; }
            get { return this.m_Mapping.PropertyName; }
        }
        /// <summary>
        /// �ֶ�����
        /// </summary>
        public FieldType FieldType
        {
            set { this.m_Mapping.FieldType = value; }
            get { return this.m_Mapping.FieldType; }
        }
        /// <summary>
        /// �ֶγ���
        /// </summary>
        public int FieldLength
        {
            set { this.m_Mapping.FieldLength = value; }
            get { return this.m_Mapping.FieldLength; }
        }
        /// <summary>
        /// ע��
        /// </summary>
        public string Remark
        {
            set { this.m_Mapping.Remark = value; }
            get { return this.m_Mapping.Remark; }
        }
        #endregion

        /// <summary>
        /// ת��string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("PropertyName=\"" + FormatString(PropertyName) + "\",");
            sb.Append("ColumnName=\"" + FormatString(ColumnName) + "\",");
            sb.Append("FieldType=" + Utility.ConvertFieldTypeToString(FieldType) + ",");
            sb.Append("FieldLength=" + FieldLength + ",");
            sb.Append("Remark=\"" + FormatString(Remark)+"\"");
            return sb.ToString();
        }
        /// <summary>
        /// ���ַ�����ʽת���ɴ˸�ʽ
        /// </summary>
        /// <param name="stuExpress"></param>
        /// <returns></returns>
        public static PrimaryKeyMapping ParseTo(string stuExpress)
        {
            if (String.IsNullOrEmpty(stuExpress))
                return null;

            PrimaryKeyMapping mapping = new PrimaryKeyMapping();
            string[] atts = stuExpress.Split(',');
            if (atts == null || atts.Length == 0)
                return mapping;
            foreach (string att in atts)
            {
                if (att.Length == 0) continue;
                string[] nmVal = att.Split('=');
                if (nmVal == null || nmVal.Length != 2)
                    continue;
                string name = nmVal[0].Trim().ToUpper();
                string val = nmVal[1].Trim();
                switch (name)
                {
                    case "PROPERTYNAME": mapping.PropertyName = val; break;
                    case "COLUMNNAME": mapping.ColumnName = val; break;
                    case "FIELDTYPE": mapping.FieldType = Utility.ParseFieldType(val); break;
                    case "FIELDLENGTH": { int l = 0; Int32.TryParse(val, out l); mapping.FieldLength = l; break; }
                    case "REMARK": mapping.Remark = val; break;
                }
            }
            return mapping;
        }
        private static string FormatString(string valStr)
        {
            if (String.IsNullOrEmpty(valStr))
                return string.Empty;
            return valStr.Replace("\"", "\\\"").Replace("\n", "\\\n");
        }
    }
}
