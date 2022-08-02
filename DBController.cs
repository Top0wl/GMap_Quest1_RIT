using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMapQuest1
{
    public class DBController
    {
        private static DBController instance;
        private SqlConnection sqlConnection;
        public DBController() { }
        public static DBController getInstance()
        {
            if (instance == null)
                instance = new DBController();
            return instance;
        }
        public void OpenConnection()
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseMarkers"].ConnectionString);
            sqlConnection.Open();
            if (sqlConnection.State == ConnectionState.Closed)
            {
                throw new Exception("Неудалось подключиться к базе данных");
            }
        }
        public void CloseConnection()
        {
            sqlConnection.Close();
            if (sqlConnection.State == ConnectionState.Open)
            {
                throw new Exception("Неудалось закрыть подлкючение к базе данных");
            }
        }
        public void LoadMarkers(GMapControl gMapControl)
        {
            this.OpenConnection();
            GMapOverlay markersOverlay = new GMapOverlay("DB markers");
            SqlCommand cmd = new SqlCommand("Select Latitude, Longitude, Name, Id from Markers", sqlConnection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PointLatLng latlng = new GMap.NET.PointLatLng(
                    Convert.ToDouble(reader["Latitude"]),
                    Convert.ToDouble(reader["Longitude"]));
                string name = Convert.ToString(reader["Name"]);
                int id = Convert.ToInt32(reader["Id"]);
                CustomMarker marker = new CustomMarker(latlng, GMarkerGoogleType.red, name, id);
                markersOverlay.Markers.Add(marker);
            }
            gMapControl.Overlays.Add(markersOverlay);
            this.CloseConnection();
        }
        public void RefreshPositionMarker(CustomMarker marker)
        {
            this.OpenConnection();

            SqlCommand cmd = new SqlCommand($"UPDATE Markers SET Latitude = @Latitude, Longitude = @Longitude WHERE Id = @Id", sqlConnection);
            cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = marker.Position.Lat;
            cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = marker.Position.Lng;
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = marker.Id;
            var q = cmd.ExecuteNonQuery();
            if (q == 1)
            {
                MessageBox.Show($"Положение точки {marker.ToolTipText} успешно изменино");
            }
            else 
            {
                throw new Exception($"Не удалось изменить позицию точки {marker.ToolTipText}");
            }

            this.CloseConnection();
        }
        public int AddMarker(string name, GMap.NET.PointLatLng latlng)
        {
            this.OpenConnection();

            SqlCommand cmd = new SqlCommand($"Insert into Markers output INSERTED.ID Values (@Latitude, @Longitude, @Name)", sqlConnection);
            cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = latlng.Lat;
            cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = latlng.Lng;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
            int inserted_id = (int)cmd.ExecuteScalar();

            this.CloseConnection();
            return inserted_id;
        }
    }
}
