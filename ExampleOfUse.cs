using System;
using Microsoft.ApplicationBlocks.Data;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CadCards
{
....

		public static DataSet GetCadCrd ( Int32 intOperationId, Int32 intUserSecurityId, Object strFromDate, Object strToDate, Object strRunNo)
		{
			// ( strFromDate and strToDate ) and strRunNo are mutually exclusive
			try
			{
				// check UseCODS flag
				if (ConfigurationSettings.AppSettings["UseCODS"].ToLower() == "yes")
				{
					SqlParameter[] cmdNoParam	= {};
					string strSQL = "";

					// get operation name
					strSQL = string.Format(@"
SMALL SQL SELECT STATEMENT HERE ", intOperationId.ToString() );

					DataSet objDS = SqlHelper.ExecuteDataset(GetConn()
													, CommandType.Text
													, strSQL
													, cmdNoParam);

					string strOperationName = "";
					if ( objDS.Tables.Count > 0 )
						if ( objDS.Tables[ 0 ].Rows.Count > 0 )
							strOperationName = objDS.Tables[ 0 ].Rows[ 0 ][ "OperationName" ].ToString();

					// load priority codes/descriptions
					strSQL = @"
SMALL SQL SELECT STATEMENT HERE ";
					DataSet objPriorityDS = SqlHelper.ExecuteDataset(GetConn()
													, CommandType.Text
													, strSQL
													, cmdNoParam );

					// if RunNbr is specified, include it as criteria
					strSQL	= string.Format(@"
HUGE SQL SELECT STATEMENT HERE 
", strOperationName
						 , strRunNo.ToString());

					objDS	= SqlHelper.ExecuteDataset( GetCODSConn(), CommandType.Text, strSQL, cmdNoParam );

					if( objDS.Tables.Count > 0 )
						objDS.Tables[0].TableName = "GetCADCard";

					// populate each priority type from the CAD_Historic database
					foreach (DataRow objDR in objDS.Tables[ 0 ].Rows)
					{
						string strSearch	= string.Format( "Priority = '{0}'", objDR[ "Priority" ].ToString() );
						DataRow[]	objPriorityRows	= objPriorityDS.Tables[0].Select( strSearch );
						if ( objPriorityRows.GetLength( 0 ) > 0 )
							objDR["PriorityType"] = objPriorityRows[0]["PriorityDescription"].ToString();
					}

					// add image column to the dataset for the barcode
					objDS.Tables[0].Columns.Add( "BarCode", typeof( System.Byte[] ) );
					BarCode39.Weight = BarCode39.BarCodeWeight.Medium;
					BarCode39.LeftMargin = 5;
					BarCode39.VertAlign = BarCode39.AlignType.Left;
					BarCode39.BarCodeHeight = 120;
					foreach (DataRow objDR in objDS.Tables[0].Rows)
						objDR["BarCode"] = BarCode39.CreateBarCodePNGByteArray(objDR["Company"].ToString(), 4);

					return objDS;
				}

				// this is probably where the changes must be made to retrieve data from CODS instead

				SqlParameter[] cmdParam = { 
												new SqlParameter("@OperationId", intOperationId) 
												,new SqlParameter("@UserSecurityId", intUserSecurityId)
												,new SqlParameter("@FromDate", strFromDate)
												,new SqlParameter("@ToDate", strToDate)
												,new SqlParameter("@RunNo", strRunNo)
										  }; 

				return SqlHelper.ExecuteDataset(GetConn(), CommandType.StoredProcedure, "GetCadCard", cmdParam);
			}
			catch ( Exception e)
			{
				throw e;
			}
		}
}
