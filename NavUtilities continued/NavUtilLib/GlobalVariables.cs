//NavUtilities by kujuman, © 2014. All Rights Reserved.

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using KSP;
using FinePrint;

namespace NavUtilLib
{
    namespace GlobalVariables
    {
        public static class Settings
        {

			public static string getPathFor(string subdir) {
				return KSP.IO.IOUtils.GetFilePathFor(typeof(Settings), subdir, null)
					.Replace("/", System.IO.Path.DirectorySeparatorChar.ToString());
			}

			public static string getPluginDataParentPath(string subdir) {
				string fullPath = getPathFor("JustAnAnchor");
				return fullPath.Substring(0, fullPath.IndexOf("PluginData"))
					+ subdir + System.IO.Path.DirectorySeparatorChar.ToString();
			}

			public static string getCustomRunwaysFile() {
				return getPluginDataParentPath("Runways")
					+ "customRunways.cfg";
			}

			public static string getLauncherTextureFile() {
				return getPluginDataParentPath("Textures/Toolbar")
					+ "toolbarButton3838.png";
			}

			private static string getPathRelativeToGameData(string subdir) {
				string path = getPluginDataParentPath(subdir);
				string startOfRelative = System.IO.Path.DirectorySeparatorChar.ToString() + "gamedata" + System.IO.Path.DirectorySeparatorChar.ToString();
				int ind = path.ToLower().LastIndexOf(startOfRelative);
				path = path.Substring(ind + startOfRelative.Length).Replace("\\", "/");
				return path;
			}

			//Blizzy's toolbar plugin wants texture path relative to GameData, not in PluginData, separated by / and without extension
			public static string getToolbarTextureFile() {
				return getPathRelativeToGameData("Textures/Toolbar") + "toolbarButton";
			}

			public static string getAudioPath() {
				return getPathRelativeToGameData("Audio");
			}

			public static string settingsFileURL = getPathFor("settings.cfg");
            //public static string gsFileURL = "GameData/KerbalScienceFoundation/NavInstruments/glideslopes.cfg";

            public static bool isKSPGUIActive = true;

            public static Rect hsiPosition = new Rect(50,50,640,640);
            public static float hsiGUIscale = 0.5f;
            public static bool hsiState = false;

            public static Rect settingsGUI = new Rect(100,50,250,180);

            public static Rect rwyEditorGUI = new Rect(50, 50, 450, 300);
            public static bool rwyEditorState = false;

            public static bool navAidsIsLoaded = false;

            public static bool enableFineLoc = true;

            public static bool loadCustom_rwyCFG = true;
            public static bool useBlizzy78ToolBar = false;

            public static bool enableWindowsInIVA = true;

			public static bool hideNavBallWaypoint = false;

            public static int appInstance;

            public static NavUtilLibApp appReference;

            public static bool enableDebugging = false;

			/*public static void loadNavAids_not_working() {
				FlightData.rwyList.Clear();
				ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes("Runway");
				KSPLog.print("^^^^ NODES: " + nodes.Length);
				foreach (ConfigNode node in nodes) {
					Runway runway = new Runway();
					ConfigNode.LoadObjectFromConfig(runway, node);
					KSPLog.print("^^^^ RUNWAY: " + runway);
					FlightData.rwyList.Add(runway);
				}

				nodes = GameDatabase.Instance.GetConfigNodes("Glideslope");
				KSPLog.print("^^^^ GLIDE NODES: " + nodes.Length);
				foreach (ConfigNode node in nodes) {
					KSPLog.print("^^^^ GLIDE NODE: " + node);
					Glideslope glideslope = new Glideslope();
					KSPLog.print("^^^^ LOADED " + ConfigNode.LoadObjectFromConfig(glideslope, node));
					KSPLog.print("^^^^ GLIDESLOPE: " + glideslope);
					FlightData.gsList.Add(glideslope);
				}
			}*/

            public static void loadNavAids()
            {
                if (enableDebugging) Debug.Log("NavUtil: Loading NavAid database...");
                FlightData.allRunways.Clear();
                FlightData.allRunways = ConfigLoader.GetRunwayListFromConfig();
                FlightData.gsList.Clear();
                FlightData.gsList = ConfigLoader.GetGlideslopeListFromConfig();

                FlightData.customRunways.Clear();
				FlightData.allRunways.ForEach(runway => {
					if (FlightGlobals.currentMainBody == null) {
						return;
					}
					if (FlightGlobals.currentMainBody.name == runway.body) {
						FlightData.currentBodyRunways.Add(runway);
					}
					if (runway.custom) {
						FlightData.customRunways.Add(runway);
					}
				});

                /*DirectoryInfo folder = new DirectoryInfo(KSPUtil.ApplicationRootPath + "GameData/KerbalScienceFoundation/NavInstruments/Runways");

                if (folder.Exists)
                {
                    //FileInfo[] addlNavAidFiles = folder.GetFiles("*.rwy"); //this works great :D
                    FileInfo[] addlNavAidFiles = folder.GetFiles("*");


                    foreach (FileInfo f in addlNavAidFiles)
                    {
                        if ((f.Name.EndsWith("_rwy.cfg") && loadCustom_rwyCFG)|| f.Name.EndsWith(".rwy"))
                        {

                            if (enableDebugging)  Debug.Log("NavUtil: found file " + f.Name.ToString());

                            if (f.Name == "custom.rwy" || (f.Name == "custom_rwy.cfg" && GlobalVariables.Settings.loadCustom_rwyCFG))
                            {
                                FlightData.customRunways.AddRange(NavUtilLib.ConfigLoader.GetRunwayListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + f.Name));
                                if (enableDebugging)  Debug.Log("NavUtil: Found " + f.Name + " with " + FlightData.customRunways.Count + " runway definitions");

                            }

                            if (enableDebugging) Debug.Log("NavUtil: Found " + f.Name);

                            FlightData.rwyList.AddRange(NavUtilLib.ConfigLoader.GetRunwayListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + f.Name));
                            //! FlightData.gsList.AddRange(NavUtilLib.ConfigLoader.GetGlideslopeListFromConfig("GameData/KerbalScienceFoundation/NavInstruments/Runways/" + f.Name));
                            //}
                        }
                    }
                }*/

                navAidsIsLoaded = true;
            }
        }

        public static class FlightData
        {
			public static List<Runway> allRunways = new List<Runway>();
			public static List<Runway> currentBodyRunways = new List<Runway>();
            public static int rwyIdx;

			public static List<float> gsList = new List<float>();
            public static int gsIdx;

            public static List<Runway> customRunways = new List<Runway>();
            public static int cRwyIdx;

            public static Runway selectedRwy;
            public static float selectedGlideSlope;
            public static Vessel currentVessel;
			public static CelestialBody currentBody = null;
            /// <summary>
            /// /////////
            /// </summary>
            private static double lastNavUpdateUT;
            public static void SetLastNavUpdateUT()
            {
                lastNavUpdateUT = Planetarium.GetUniversalTime();
            }
            public static double GetLastNavUpdateUT()
            {
                return lastNavUpdateUT;
            }

            public static double bearing;
            public static double dme;
            public static double elevationAngle;
            public static float locDeviation;
            public static float gsDeviation;
            public static float runwayHeading;

			public static bool fallback = false;

			private static Waypoint prevWaypoint = null;
            
            public static bool isINSMode() {
            	return (selectedRwy != null) && selectedRwy.isINSTarget;
            }

            public static void updateNavigationData()
            {
                //see if information is current
                if (GetLastNavUpdateUT() != Planetarium.GetUniversalTime())
                {
					if (currentBody == null || FlightGlobals.currentMainBody != currentBody) {
						rwyIdx = 0;
						currentBody = FlightGlobals.currentMainBody;
						currentBodyRunways.Clear();
						for (int i = 0; i < allRunways.Count; i++) {
							if (allRunways[i].body == currentBody.name) {
								currentBodyRunways.Add(allRunways[i]);
							}
						}
					}
                    selectedGlideSlope = gsList[gsIdx];
					if (currentBodyRunways.Count == 0) {
						selectedRwy = null;
						rwyIdx = 0;
						fallback = true;
					} else {
						selectedRwy = currentBodyRunways[rwyIdx];
						fallback = false;
					}
                    

                    //Since there seems to be no callback methods to determine whether waypoint has been set or changed, we have to refresh INS data on every update  
					NavWaypoint navWaypoint = NavWaypoint.fetch;
					if ((navWaypoint != null) && navWaypoint.IsActive && navWaypoint.Body == FlightGlobals.currentMainBody) {
						Waypoint waypoint = null;
						if (prevWaypoint != null && navWaypoint.IsUsing(prevWaypoint)) {
							waypoint = prevWaypoint;
						} else {
							foreach (Waypoint wp in FinePrint.WaypointManager.Instance().Waypoints) {
								if (navWaypoint.IsUsing(wp)) {
									waypoint = wp;
									break;
								}
							}
							prevWaypoint = waypoint;
						}

                        //If waypoint is fine then generate fake target runway every time
                        Runway insTarget = new Runway();
                        insTarget.isINSTarget = true;
						insTarget.ident = waypoint != null ? waypoint.name : navWaypoint.name;
                        insTarget.body = navWaypoint.Body.name;
                        insTarget.hdg = selectedRwy != null ? selectedRwy.hdg : 0;
                        insTarget.altMSL = (float)(navWaypoint.Height + navWaypoint.Altitude);
                        insTarget.locLatitude = (float)navWaypoint.Latitude;
                        insTarget.locLongitude = (float)navWaypoint.Longitude;
                        insTarget.gsLatitude = (float)navWaypoint.Latitude;
                        insTarget.gsLongitude = (float)navWaypoint.Longitude;
                        selectedRwy = insTarget;
                    }

                    currentVessel = FlightGlobals.ActiveVessel;
					if (selectedRwy != null) {
						bearing = NavUtilLib.Utils.CalcBearingToBeacon(currentVessel, selectedRwy);
						dme = NavUtilLib.Utils.CalcDistanceToBeacon(currentVessel, selectedRwy);
						elevationAngle = NavUtilLib.Utils.CalcElevationAngle(currentVessel, selectedRwy);
						//locDeviation = NavUtilLib.Utils.CalcLocalizerDeviation(bearing, selectedRwy);
						locDeviation = (float)NavUtilLib.Utils.CalcLocalizerDeviation(currentVessel, selectedRwy);
						gsDeviation = NavUtilLib.Utils.CalcGlideslopeDeviation(elevationAngle, selectedGlideSlope);

						//
						runwayHeading = (float)NavUtilLib.Utils.CalcProjectedRunwayHeading(currentVessel, selectedRwy);
					} else {
						bearing = 0;
						dme = 0;
						elevationAngle = 0;
						locDeviation = 0;
						gsDeviation = 0;
						runwayHeading = 0;
						selectedRwy = Runway.fallback();
					}

                    SetLastNavUpdateUT();
                }
            }
        }


        public class Materials
        {
            private static Materials instance;
            private Materials() { }

            public static Materials Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new Materials();
                    }
                    return instance;
                }
            }

            public static bool isLoaded = false;
            public Material overlay = null;
            public Material pointer = null;
            public Material headingCard = null;
            public Material NDBneedle = null;
            public Material course = null;
            public Material localizer = null;
            public Material mkrbcn = null;
            public Material flag = null;
            public Material back = null;
            public Material whiteFont = null;

            public Material AI_overlay = null;
            public Material AI_throttleBar = null;
            public Material AI_VSILine = null;
            public Material AI_Ladder = null;
            public Material AI_Radar = null;
            public Material AI_RadarDial = null;

            public static void loadMaterials()
            {
                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtilLib: Updating materials...");
                string texName;
                texName = "hsi_overlay.png";
				Materials.Instance.overlay = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.overlay, 640, 640);

				texName = "hsi_gs_pointer.png";
				Materials.Instance.pointer = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.pointer, 640, 24);

				texName = "hsi_large_heading_card.png";
				Materials.Instance.headingCard = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.headingCard, 501, 501);

				texName = "hsi_NDB_needle.png";
				Materials.Instance.NDBneedle = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.NDBneedle, 15, 501);

				texName = "hsi_course_needle.png";
				Materials.Instance.course = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.course, 221, 481);

				texName = "hsi_course_deviation_needle.png";
				Materials.Instance.localizer = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.localizer, 5, 251);

				texName = "hsi_markerIndicator.png";
				Materials.Instance.mkrbcn = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.mkrbcn, 175, 180);

				texName = "hsi_flags.png";
				Materials.Instance.flag = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.flag, 64, 64);

				texName = "hsi_back.png";
				Materials.Instance.back = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.back, 32, 32);

				texName = "white_font.png";
				Materials.Instance.whiteFont = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.whiteFont, 256, 256);

				texName = "AI_OVERLAY.png";
				Materials.Instance.AI_overlay = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.AI_overlay, 640, 640);

				texName = "AI_THROTTLEBAR.png";
				Materials.Instance.AI_throttleBar = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.AI_throttleBar,27, 164);

				texName = "AI_VSILINE.png";
				Materials.Instance.AI_VSILine = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.AI_VSILine, 33, 4);

				texName = "AI_LADDER.png";
				Materials.Instance.AI_Ladder = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.AI_Ladder, 906, 2048);
                
				texName = "AI_RADAR.png";
				Materials.Instance.AI_Radar = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.AI_Radar, 179, 179);

				texName = "AI_RADARDIAL.png";
				Materials.Instance.AI_RadarDial = NavUtilGraphics.loadMaterial(GlobalVariables.Settings.getPluginDataParentPath("Textures") +  texName, Materials.Instance.AI_RadarDial, 86, 39);

                isLoaded = true;
            }
        }

        public class Audio
        {
            private static Audio instance;
            private Audio() { }

            public static Audio Instance
            {
                get
                {
                    if(instance == null)
                    {
                        instance = new Audio();
                    }
                    return instance;
                }
            }

            public static bool isLoaded = false;
            
            public static GameObject audioplayer; 
            private static AudioSource markerAudio;
            //public static AudioSource playOnce;

            private bool isPlaying = false;

            public static void initializeAudio()
            {
                audioplayer = new GameObject();
                markerAudio = new AudioSource();
                //playOnce = new AudioSource();

                if (GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtilLib: InitializingAudio...");

                try
                {
                markerAudio = audioplayer.AddComponent<AudioSource>();
                markerAudio.volume = GameSettings.UI_VOLUME;
                markerAudio.panStereo = 0;
                markerAudio.dopplerLevel = 0;
                markerAudio.bypassEffects = true;
                markerAudio.loop = true;
                markerAudio.rolloffMode = AudioRolloffMode.Linear;
                //markerAudio.transform.SetParent(FlightCamera.fetch.mainCamera.transform);

                //playOnce = audioplayer.AddComponent<AudioSource>();
                //playOnce.volume = GameSettings.VOICE_VOLUME;
                //playOnce.pan = 0;
                //playOnce.dopplerLevel = 0;
                //playOnce.bypassEffects = true;
                //playOnce.loop = false;
                //playOnce.rolloffMode = AudioRolloffMode.Linear;
                //playOnce.transform.SetParent(FlightCamera.fetch.mainCamera.transform);

                
                }
                catch (Exception)
                {
                    if (NavUtilLib.GlobalVariables.Settings.enableDebugging) Debug.Log("NavUtil: Error Loading Audio");

                    throw;
                }

                isLoaded = true;
            }

            public static void PlayClick()
            {
                Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip(Settings.getAudioPath() + "click"));
            }

            public void PlayOuter()
            {
                Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip(Settings.getAudioPath() + "outer"));
                this.isPlaying = true;
            }

            public void PlayMiddle()
            {
                Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip(Settings.getAudioPath() + "middle"));
                this.isPlaying = true;
            }

            public void PlayInner()
            {
                Audio.markerAudio.PlayOneShot(GameDatabase.Instance.GetAudioClip(Settings.getAudioPath() + "inner"));
                this.isPlaying = true;
            }

            public void Stop()
            {
                if (this.isPlaying)
                {
                    Audio.markerAudio.Stop();
                    this.isPlaying = false;
                }
            }
        }
    }
}

