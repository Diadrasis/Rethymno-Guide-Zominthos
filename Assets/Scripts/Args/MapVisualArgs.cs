//Diadrasis ©2023 - Stathis Georgiou
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Diadrasis.Rethymno 
{
	[Serializable]
	public class MapVisualArgs
	{
		public bool applyVisualSettings;
		[Space]
		//snazzy maps
		//public string snazzyMapsProviderURL = "https://maps.googleapis.com/maps/vt?pb=!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!2m3!1e0!2sm!3i480190004!3m14!2sen-US!3sUS!5e18!12m1!1e68!12m3!1e37!2m1!1ssmartmaps!12m4!1e26!2m2!1sstyles!2zcy5lOmwudC5mfHAuczozNnxwLmM6I2ZmMzMzMzMzfHAubDo0MCxzLmU6bC50LnN8cC52Om9ufHAuYzojZmZmZmZmZmZ8cC5sOjE2LHMuZTpsLml8cC52Om9mZixzLnQ6MXxzLmU6Z3xwLnY6b24scy50OjF8cy5lOmcuZnxwLmM6I2ZmZmVmZWZlfHAubDoyMCxzLnQ6MXxzLmU6Zy5zfHAuYzojZmZmZWZlZmV8cC5sOjE3fHAudzoxLjIscy50OjF8cy5lOmx8cC52Om9mZixzLnQ6MXxzLmU6bC5pfHAudjpvZmYscy50OjE3fHMuZTpsfHAudjpvZmYscy50OjE4fHMuZTpsfHAudjpvZmYscy50OjE5fHMuZTpsfHAudjpvZmYscy50OjIwfHMuZTpsfHAudjpvZmYscy50OjIxfHMuZTpsfHAudjpvZmYscy50OjV8cy5lOmd8cC5jOiNmZmY1ZjVmNXxwLmw6MjAscy50OjgxfHMuZTpsfHAudjpvZmYscy50OjEzMTN8cy5lOmx8cC52Om9mZixzLnQ6MTMxNHxzLmU6bHxwLnY6b2ZmLHMudDoyfHMuZTpnfHAuYzojZmZmNWY1ZjV8cC5sOjIxLHMudDo0MHxzLmU6Z3xwLmM6I2ZmZGVkZWRlfHAubDoyMSxzLnQ6NDl8cy5lOmcuZnxwLmM6I2ZmZmZmZmZmfHAubDoxNyxzLnQ6NDl8cy5lOmcuc3xwLmM6I2ZmZmZmZmZmfHAubDoyOXxwLnc6MC4yLHMudDo1MHxzLmU6Z3xwLmM6I2ZmZmZmZmZmfHAubDoxOCxzLnQ6NTF8cy5lOmd8cC5jOiNmZmZmZmZmZnxwLmw6MTYscy50OjR8cy5lOmd8cC5jOiNmZmYyZjJmMnxwLmw6MTkscy50OjZ8cy5lOmd8cC5jOiNmZmU5ZTllOXxwLmw6MTc!4e0";

		//custom th-ink site
		//https://maps.googleapis.com/maps/vt?pb=!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!2m3!1e4!2st!3i{y}637!2m3!1e0!2sr!3i{y}637375603!3m17!2sel!3sUS!5e18!12m4!1e68!2m2!1sset!2sTerrain!12m3!1e37!2m1!1ssmartmaps!12m4!1e26!2m2!1sstyles!2zcy5lOmd8cC5jOiNlYmUzY2Qscy5lOmx8cC52Om9mZixzLmU6bC50LmZ8cC5jOiM1MjM3MzUscy5lOmwudC5zfHAuYzojZjVmMWU2LHMudDoxfHMuZTpnLnN8cC5jOiNjOWIyYTYscy50OjIxfHAudjpvZmYscy50OjIxfHMuZTpnLnN8cC5jOiNkY2QyYmUscy50OjIxfHMuZTpsLnQuZnxwLmM6I2FlOWU5MCxzLnQ6MjB8cC52Om9mZixzLnQ6ODJ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjJ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjJ8cy5lOmwudC5mfHAuYzojOTM4MTdjLHMudDo0MHxzLmU6Zy5mfHAuYzojYTViMDc2LHMudDo0MHxzLmU6bC50LmZ8cC5jOiM0NDc1MzAscy50OjN8cC52Om9mZixzLnQ6M3xzLmU6Z3xwLmM6I2Y1ZjFlNixzLnQ6NTB8cy5lOmd8cC5jOiNmZGZjZjgscy50OjQ5fHMuZTpnfHAuYzojZjhjOTY3LHMudDo0OXxzLmU6Zy5zfHAuYzojZTliYzYyLHMudDo3ODV8cy5lOmd8cC5jOiNlOThkNTgscy50Ojc4NXxzLmU6Zy5zfHAuYzojZGI4NTU1LHMudDo1MXxzLmU6bC50LmZ8cC5jOiM4MDZiNjMscy50OjY1fHMuZTpnfHAuYzojZGZkMmFlLHMudDo2NXxzLmU6bC50LmZ8cC5jOiM4ZjdkNzcscy50OjY1fHMuZTpsLnQuc3xwLmM6I2ViZTNjZCxzLnQ6NjZ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjZ8cy5lOmcuZnxwLmM6I2I5ZDNjMixzLnQ6NnxzLmU6bC50LmZ8cC5jOiM5Mjk5OGQ!4e0
		//source url was >> https://maps.googleapis.com/maps/vt?pb=!1m4!1m3!1i15!2i18539!3i12641!1m4!1m3!1i15!2i18539!3i12642!1m4!1m3!1i15!2i18539!3i12643!1m4!1m3!1i15!2i18540!3i12641!1m4!1m3!1i15!2i18541!3i12641!1m4!1m3!1i15!2i18540!3i12642!1m4!1m3!1i15!2i18540!3i12643!1m4!1m3!1i15!2i18541!3i12642!1m4!1m3!1i15!2i18541!3i12643!1m4!1m3!1i15!2i18542!3i12641!1m4!1m3!1i15!2i18543!3i12641!1m4!1m3!1i15!2i18542!3i12642!1m4!1m3!1i15!2i18542!3i12643!1m4!1m3!1i15!2i18543!3i12642!1m4!1m3!1i15!2i18543!3i12643!1m4!1m3!1i15!2i18544!3i12641!1m4!1m3!1i15!2i18545!3i12641!1m4!1m3!1i15!2i18544!3i12642!1m4!1m3!1i15!2i18544!3i12643!1m4!1m3!1i15!2i18545!3i12642!1m4!1m3!1i15!2i18545!3i12643!1m4!1m3!1i15!2i18546!3i12641!1m4!1m3!1i15!2i18547!3i12641!1m4!1m3!1i15!2i18546!3i12642!1m4!1m3!1i15!2i18546!3i12643!2m3!1e4!2st!3i637!2m3!1e0!2sr!3i637375603!3m17!2sel!3sUS!5e18!12m4!1e68!2m2!1sset!2sTerrain!12m3!1e37!2m1!1ssmartmaps!12m4!1e26!2m2!1sstyles!2zcy5lOmd8cC5jOiNlYmUzY2Qscy5lOmx8cC52Om9mZixzLmU6bC50LmZ8cC5jOiM1MjM3MzUscy5lOmwudC5zfHAuYzojZjVmMWU2LHMudDoxfHMuZTpnLnN8cC5jOiNjOWIyYTYscy50OjIxfHAudjpvZmYscy50OjIxfHMuZTpnLnN8cC5jOiNkY2QyYmUscy50OjIxfHMuZTpsLnQuZnxwLmM6I2FlOWU5MCxzLnQ6MjB8cC52Om9mZixzLnQ6ODJ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjJ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjJ8cy5lOmwudC5mfHAuYzojOTM4MTdjLHMudDo0MHxzLmU6Zy5mfHAuYzojYTViMDc2LHMudDo0MHxzLmU6bC50LmZ8cC5jOiM0NDc1MzAscy50OjN8cC52Om9mZixzLnQ6M3xzLmU6Z3xwLmM6I2Y1ZjFlNixzLnQ6NTB8cy5lOmd8cC5jOiNmZGZjZjgscy50OjQ5fHMuZTpnfHAuYzojZjhjOTY3LHMudDo0OXxzLmU6Zy5zfHAuYzojZTliYzYyLHMudDo3ODV8cy5lOmd8cC5jOiNlOThkNTgscy50Ojc4NXxzLmU6Zy5zfHAuYzojZGI4NTU1LHMudDo1MXxzLmU6bC50LmZ8cC5jOiM4MDZiNjMscy50OjY1fHMuZTpnfHAuYzojZGZkMmFlLHMudDo2NXxzLmU6bC50LmZ8cC5jOiM4ZjdkNzcscy50OjY1fHMuZTpsLnQuc3xwLmM6I2ViZTNjZCxzLnQ6NjZ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjZ8cy5lOmcuZnxwLmM6I2I5ZDNjMixzLnQ6NnxzLmU6bC50LmZ8cC5jOiM5Mjk5OGQ!4e3!12m1!5b1!23i1379903&callback=_xdc_._j0aznx&key=AIza

		[TextAreaAttribute(3, 10)]
		public string customProviderURL = "https://maps.googleapis.com/maps/vt?pb=!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!1m4!1m3!1i{zoom}!2i{x}!3i{y}!2m3!1e4!2st!3i{y}637!2m3!1e0!2sr!3i{y}637375603!3m17!2sel!3sUS!5e18!12m4!1e68!2m2!1sset!2sTerrain!12m3!1e37!2m1!1ssmartmaps!12m4!1e26!2m2!1sstyles!2zcy5lOmd8cC5jOiNlYmUzY2Qscy5lOmx8cC52Om9mZixzLmU6bC50LmZ8cC5jOiM1MjM3MzUscy5lOmwudC5zfHAuYzojZjVmMWU2LHMudDoxfHMuZTpnLnN8cC5jOiNjOWIyYTYscy50OjIxfHAudjpvZmYscy50OjIxfHMuZTpnLnN8cC5jOiNkY2QyYmUscy50OjIxfHMuZTpsLnQuZnxwLmM6I2FlOWU5MCxzLnQ6MjB8cC52Om9mZixzLnQ6ODJ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjJ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjJ8cy5lOmwudC5mfHAuYzojOTM4MTdjLHMudDo0MHxzLmU6Zy5mfHAuYzojYTViMDc2LHMudDo0MHxzLmU6bC50LmZ8cC5jOiM0NDc1MzAscy50OjN8cC52Om9mZixzLnQ6M3xzLmU6Z3xwLmM6I2Y1ZjFlNixzLnQ6NTB8cy5lOmd8cC5jOiNmZGZjZjgscy50OjQ5fHMuZTpnfHAuYzojZjhjOTY3LHMudDo0OXxzLmU6Zy5zfHAuYzojZTliYzYyLHMudDo3ODV8cy5lOmd8cC5jOiNlOThkNTgscy50Ojc4NXxzLmU6Zy5zfHAuYzojZGI4NTU1LHMudDo1MXxzLmU6bC50LmZ8cC5jOiM4MDZiNjMscy50OjY1fHMuZTpnfHAuYzojZGZkMmFlLHMudDo2NXxzLmU6bC50LmZ8cC5jOiM4ZjdkNzcscy50OjY1fHMuZTpsLnQuc3xwLmM6I2ViZTNjZCxzLnQ6NjZ8cy5lOmd8cC5jOiNkZmQyYWUscy50OjZ8cy5lOmcuZnxwLmM6I2I5ZDNjMixzLnQ6NnxzLmU6bC50LmZ8cC5jOiM5Mjk5OGQ!4e0";

		[Space]
		public EnumsHolder.MapsProvider mapsProvider;

	}

}