using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Prometheus.Model
{
    public class Camera
    {
        [Key]
        public Guid CameraId { get; set; }

        public string CameraStreamUrl { get; set; }

        public string CameraDescription { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser CameraHolder { get; set; }

        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }
    }
}
