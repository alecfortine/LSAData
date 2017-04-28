using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

namespace LSAData
{
    public static class LSData
    {
        private static string strSQLCon = @"Data Source=AFORTINE\SQLEXPRESS;Initial Catalog=LaborNeedsScheduling;Integrated Security=True;";

        private static DataTable dt = new DataTable();


        /// <summary>
        /// Fills the initial table with data from the source.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="weekMarker"></param>
        /// <returns></returns>
        public static DataTable FillWTGTable(double[] weights, DateTime weekMarker)
        {

            using (SqlConnection conn = new SqlConnection(strSQLCon))
            {

                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    conn.Open();

                    for (int i = 0; i < weights.Length; i++)
                    {
                        if (i == 0)
                        {
                            cmd.CommandText = "Select FORMAT([Date], 'M/d/yyyy') as Date, [WeekDay], HourofDay,"
                            + "TrafficOut, Round((TrafficOut * " + weights[i] + "),2) as WTGTraffic From Backup_LastSixWeeksTraffic "
                            + "Where[Date] < dateadd(WEEK," + (-1 * (weights.Length - 1 - i)).ToString() + ", '" + weekMarker.ToShortDateString() + "')";
                        }
                        else if (i < weights.Length - 1)
                        {
                            cmd.CommandText = "Select FORMAT([Date], 'M/d/yyyy') as Date, [WeekDay], HourofDay,"
                                + "TrafficOut, Round((TrafficOut * " + weights[i] + "),2) as WTGTraffic From Backup_LastSixWeeksTraffic "
                                + "Where[Date] < dateadd(WEEK," + (-1 * (weights.Length - 1 - i)).ToString() + ", '" +
                                weekMarker.ToShortDateString() + "') AND[Date] >= dateadd(WEEK, " + (-1 * (weights.Length - i)).ToString() +
                                ", '" + weekMarker.ToShortDateString() + "')";
                        }
                        else
                        {
                            cmd.CommandText = "Select FORMAT([Date], 'M/d/yyyy') as Date, [WeekDay], HourofDay,"
                                + "TrafficOut, Round((TrafficOut * " + weights[i] + "),2) as WTGTraffic From Backup_LastSixWeeksTraffic "
                                + "Where[Date] >= dateadd(WEEK," + (-1).ToString() + ", '" + weekMarker.ToShortDateString() + "')";
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                        da.Fill(dt);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

            return dt;
        }


        /// <summary>
        /// Default weighting values for the past six weeks
        /// </summary>
        public static double[] getDefaultWeights(int numberOfWeeks)
        {
            double[] defaultWeightingValues = { .36, .24, .16, .12, .08, .04 };

            return defaultWeightingValues;
        }

        //public static string[] getHoursOfDay()
        //{
        //    string[] hoursOfDay = (select distinct HourofDay from Backup_LastSixWeeksTraffic ORDER BY HourofDay ASC).ToString();

        //    using (SqlConnection conn = new SqlConnection(strSQLCon))
        //    {

        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd.CommandType = CommandType.Text;
        //            cmd.Connection = conn;
        //            conn.Open();

        //            cmd.CommandText = "select distinct HourofDay from Backup_LastSixWeeksTraffic ORDER BY HourofDay ASC";
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);

        //            da.Fill(hoursOfDay);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //    return hoursOfDay;
        //}
    }
}
