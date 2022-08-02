using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMapQuest1
{
    public partial class Form1 : Form
    {
        private bool IsMouseDown = false;
        private CustomMarker CurrentMarker;

        public Form1()
        {
            InitializeComponent();
        }
        private void gMapControl_Load(object sender, EventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //выбор подгрузки карты – онлайн или из ресурсов
            gMapControl.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance; //какой провайдер карт используется (в нашем случае гугл) 
            //gMapControl.MapProvider = GMap.NET.MapProviders.GoogleHybridMapProvider.Instance;
            gMapControl.MinZoom = 2; //минимальный зум
            gMapControl.MaxZoom = 16; //максимальный зум
            gMapControl.Zoom = 2; // какой используется зум при открытии
            gMapControl.Position = new GMap.NET.PointLatLng(10, 10);// точка в центре карты при открытии (центр России)
            gMapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter; // как приближает (просто в центр карты или по положению мыши)
            gMapControl.CanDragMap = true; // перетаскивание карты мышью
            gMapControl.DragButton = MouseButtons.Right; // какой кнопкой осуществляется перетаскивание
            gMapControl.ShowCenter = false; //показывать или скрывать красный крестик в центре
            gMapControl.ShowTileGridLines = false; //показывать или скрывать тайлы
            DBController.getInstance().LoadMarkers(gMapControl);
        }
        private void gMapControl_MarkerLeave(GMapMarker item) => CurrentMarker = null;
        private void gMapControl_MarkerEnter(GMapMarker item) => CurrentMarker = (CustomMarker)item;
        private void gMapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //this.gMapControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl_MouseMove);
                IsMouseDown = true;
            }
        }
        private void gMapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //this.gMapControl.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.gMapControl_MouseMove);
                IsMouseDown = false;
            }
        }
        private void gMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            //Состояние когда Marker Drag
            if (IsMouseDown && CurrentMarker != null)
            {
                CurrentMarker.Drag(gMapControl.FromLocalToLatLng(e.X, e.Y));
            }
            //Состояние когда Marker Drop
            if (!IsMouseDown && CurrentMarker != null && CurrentMarker.MarkerState == CustomMarker.MarkerStates.Drag)
            {
                CurrentMarker.Drop(gMapControl.FromLocalToLatLng(e.X, e.Y));
                CurrentMarker.UpdateState();
            }
        }
        private void gMapControl_MouseDoubleClick(object sender, MouseEventArgs e) 
        {
            PointLatLng latlng = gMapControl.FromLocalToLatLng(e.X, e.Y);
            string name = Convert.ToString("Test_Points");
            int id = DBController.getInstance().AddMarker(name, latlng);
            CustomMarker marker = new CustomMarker(latlng, GMarkerGoogleType.red, name, id);
            gMapControl.Overlays[0].Markers.Add(marker);
        }
    }
}
