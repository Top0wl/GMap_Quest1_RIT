using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMapQuest1
{
    public class CustomMarker : GMarkerGoogle
    {
        public readonly int Id;
        public MarkerStates MarkerState = MarkerStates.None;
        public enum MarkerStates
        {
            None = 0,
            Drag = 1,
            Drop = 2,
        }

        public CustomMarker(PointLatLng p, GMarkerGoogleType type, string name, int Id) : base(p, type)
        {
            this.Id = Id;
            this.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(this);
            this.ToolTipText = name;
        }
        public void Drag(PointLatLng point)
        {
            MarkerState = MarkerStates.Drag;
            this.Position = point;
        }
        public void Drop(PointLatLng point)
        {
            MarkerState = MarkerStates.Drop;
            this.Position = point;
            //Записываем в базу, что координаты маркера изменились
            DBController.getInstance().RefreshPositionMarker(this);
        }
        public void UpdateState()
        {
            MarkerState = MarkerStates.None;
        }
    }
}
