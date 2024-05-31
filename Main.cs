using System.Reflection;
using VMS.TPS.Common.Model.API;
using System.Data;
using System.Windows;
using System;


// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = false)]

namespace VMS.TPS
{
    public class Script
    {
        public void Execute(ScriptContext context)
        {
            Patient patient = context.Patient;

            if(patient != null)
            {
                try
                {
                    var PatSer = GetPatientSer(patient.Id);

                    Clipboard.SetText(PatSer);

                    MessageBox.Show("Patient Namn: " + patient.FirstName + " " + patient.LastName + "\r\n" + "\r\n" + "PatientSer: " + PatSer + "\r\n" + "\r\n" + "PatientSer kopierades automatiskt till Clipboard");
                }
                catch(Exception exception)
                {
                    MessageBox.Show("Kunde inte hämta PatientSer\r\n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Öppna en patient innan du kör scriptet");
            }

        }


        static private string GetPatientSer(string patientID)
        {
            (DataTable datatable, Exception exception) = AriaInterface.Query(AriaDatabase.Clinical, @"SELECT 
	                                                        Patient.PatientSer
                                                        FROM 
	                                                        Patient
                                                        WHERE 		           
                                                            Patient.PatientId LIKE @patientID 
                                                        GROUP BY
                                                            Patient.PatientSer", ("patientID", patientID));
            if (!datatable.Rows[0].IsNull(0)) return datatable.Rows[0]["PatientSer"].ToString();
            return string.Empty;
        }
    }
}
