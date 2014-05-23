using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using V_SmashCard;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Data;
using Alabama;

namespace VPSS.Data
{
    public class SmashCardDAL : DB.BaseClass<CreatedCard>
    {
        public List<CreatedCard> List()
        {
            return DB.Entities.CreatedCard.ToList(); 
        }

        public CreatedCard CreateCard(int userID, int amount, int priceLevel,
            DateTime dateRelease, string secret, string note)
        {
            try
            {
                DateTime dateCreate = DateTime.Now;
                DateTime dateExpire = dateRelease.AddYears(2);
                int release = 0;
                if (DB.Entities.CreatedCard.Count() > 0)
                    release = DB.Entities.CreatedCard.Max(m => m.ID);
                
                CreatedCard createcard = new CreatedCard();
                createcard.UserID = userID;
                createcard.Amount = amount;
                createcard.PriceLevel = priceLevel;
                createcard.DateCreate = DateTime.Now;
                createcard.DateRelease = dateRelease;
                createcard.DateExpire = dateExpire;
                createcard.Note = note;

                createcard = Insert(createcard);

                //Tạo thẻ
                string error = InsertCard(createcard.ID, secret, release, amount, priceLevel);
                if (error != null)
                {
                    Delete(createcard.ID);
                    throw new Exception(error);
                }

                return createcard;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //return null if success;
        private string InsertCard(int createID, string secret, int release, int amount, int priceLevel)
        {
            try
            {
                string[] card = new V_SmashCard.CreateCard().Create(secret, release, amount, priceLevel);
                string[] serial = new V_SmashCard.CreateCard().CreateSerial(amount, release);

                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(Int32));
                dt.Columns.Add("Serial", typeof(string));
                dt.Columns.Add("CardChar", typeof(string));
                dt.Columns.Add("CreatedCardID", typeof(Int32));
                dt.Columns.Add("CardStatusID", typeof(Int32));
                

                for (int i = 0; i < card.Length; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["Serial"] = serial[i];
                    dr["CardChar"] = card[i];
                    dr["CardStatusID"] = 1;
                    dr["CreatedCardID"] = createID;
                    dt.Rows.Add(dr);

                    //Card c = new Card();
                    //c.Serial = serial[i];
                    //c.CardChar = card[i];
                    //c.CardStatusID = 1;
                    //c.CreatedCardID = createID;
                    //new DB.BaseClass<Card>().Insert(c);
                }
                string connString =
                    System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString; 
                // connect to SQL
                using (SqlConnection connection =
                        new SqlConnection(connString))
                {
                    // make sure to enable triggers
                    // more on triggers in next post
                    SqlBulkCopy bulkCopy =
                        new SqlBulkCopy
                        (
                        connection,
                        SqlBulkCopyOptions.TableLock |
                        SqlBulkCopyOptions.FireTriggers |
                        SqlBulkCopyOptions.UseInternalTransaction,
                        null
                        );

                    // set the destination table name
                    bulkCopy.DestinationTableName = "dbo.Card";
                    connection.Open();

                    // write the data in the "dataTable"
                    bulkCopy.WriteToServer(dt);
                    connection.Close();
                    dt.Dispose();
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}