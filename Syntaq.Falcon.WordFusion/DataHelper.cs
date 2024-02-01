using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFusion.Assembly.MailMerge {
    public static class DataHelper {

        private static SqlConnection sqlConn{
            get{

                //String env = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                //string constr = "Data Source=.;Initial Catalog=ZumeForms_Prod_2016-03-10;MultipleActiveResultSets=True;Trusted_Connection=True;Pooling=false;Connect Timeout=45;"; // System.Configuration.ConfigurationManager.ConnectionStrings[env].ConnectionString;

                String env = System.Configuration.ConfigurationManager.AppSettings["Environment"];
                string constr = System.Configuration.ConfigurationManager.ConnectionStrings[env].ConnectionString;

                SqlConnection retval = new SqlConnection(constr);
                retval.Open();
                return retval;
            }
        }

        public static Byte[] FileDocumentGet(String id, Boolean currver) {

            Byte[] retval = null;
            SqlCommand cmd = new SqlCommand();

            try {

                if (isGuid(id)) {

                    cmd.Connection = sqlConn;
                    cmd.CommandText = "FileDocumentGet";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ID", SqlDbType.UniqueIdentifier).Value = new Guid(id);
                    cmd.Parameters.Add("@CurrVersion", SqlDbType.Bit).Value = Convert.ToInt16(currver);

                    // Fill The DataSet With the Contents of the Forms
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    da.Fill(ds, "File");

                    if (ds.Tables.Count > 0) {
                        if (ds.Tables[0].Rows.Count > 0) {
                            DataRow row = ds.Tables[0].Rows[0];
                            if (row["DocTemplate"] is System.DBNull) {

                            }
                            else {
                                retval = (Byte[])row["DocTemplate"];
                            }
                        }
                    }
                }
                else {

                    id= id.Trim('\"');

                    Uri uriResult;
                    bool result = Uri.TryCreate(id, UriKind.Absolute, out uriResult)
                                  && (uriResult.Scheme == Uri.UriSchemeHttp
                                      || uriResult.Scheme == Uri.UriSchemeHttps);

                    // Retrieve document template as URL
                    if (result) {
                        using (System.Net.WebClient client = new System.Net.WebClient()) {
                            // Download data.
                            byte[] response = new System.Net.WebClient().DownloadData(id);
                            return response;
                        }
                    }
                }

            }
            finally {
                sqlConn.Dispose(); ;
            }

            return retval;
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public static Boolean isGuid(string s) {

            Guid guid;
            return Guid.TryParse(s, out guid);
        }

    }


}
