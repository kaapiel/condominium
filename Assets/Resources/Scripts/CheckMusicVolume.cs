using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using TMPro;

namespace SlimUI.ModernMenu{
	
	public class CheckMusicVolume : MonoBehaviour {
        
        private string[] servers = new string[]{
        	"https://firebasestorage.googleapis.com/v0/b/condominium-unity.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/qalenium.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/personal-aa75f.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/maanaim-signal.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/weather-ac216.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/home-finance-bedda.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/iot-test-37fd6.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/livelo-automation-app.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/maanaim-lideres.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/maanaim-relatorio.appspot.com/o/",
        	"https://firebasestorage.googleapis.com/v0/b/rota-segura.appspot.com/o/"
        };
		private string baseUrl = "https://firebasestorage.googleapis.com/v0/b/condominium-unity.appspot.com/o/";
		
		public void  Start (){
			// remember volume level from last time
			GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
			StartCoroutine(DownloadAssetBundles());
		}

		public void UpdateVolume (){
			GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
		}

		IEnumerator DownloadAssetBundles() {
			using (UnityWebRequest webRequest = UnityWebRequest.Get(baseUrl)) {
			   	webRequest.SendWebRequest();

			   	GameObject loading = GameObject.Find("Loading_Text");
				Text loadingText = loading.GetComponent<Text>();
			    while (!webRequest.isDone) {
			    	loadingText.text = "Loading Buildings: " + Math.Round(webRequest.downloadProgress * 100f, 2) + "%";
			    	yield return null;
			    }
			    loading.SetActive(false);

				if (webRequest.result == UnityWebRequest.Result.Success) {
			    	DriveFiles json = JsonUtility.FromJson<DriveFiles>(webRequest.downloadHandler.text);

					GameObject menuList = GameObject.Find("MenuOptionsVerticalLayout");
		            
		            foreach(Item jsonItem in json.items) {

						string menuOptionName = jsonItem.name;
						
						GameObject btnPrefab = Instantiate(Resources.Load("Prefabs/MenuButton")) as GameObject;
						btnPrefab.transform.SetParent(menuList.transform);
		                btnPrefab.transform.localScale = new Vector3(1, 1, 1);
		                btnPrefab.transform.localPosition = new Vector3(0, 0, 0);
		                btnPrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
		                btnPrefab.name = "Btn_" + menuOptionName;
		                
						Transform[] ts = btnPrefab.GetComponentsInChildren<Transform>();
		                ts[2].GetComponent<Text>().text = jsonItem.name;

		                Button btn = btnPrefab.GetComponent<Button>();
		                btn.onClick.AddListener(delegate {StartCoroutine(openCadFile(menuOptionName));});

		                # if UNITY_EDITOR
		    			//Texture2D texture = UnityEditor.AssetPreview.GetAssetPreview(Resources.Load("Models/" + menuOptionName.ToUpper()));
						//Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.0f, 0.0f));
						//sprite.name = "Image_" + menuOptionName;

		                //Transform[] ts = btnPrefab.GetComponentsInChildren<Transform>();
		                //ts[0].GetComponent<Image>().sprite = sprite;
		                //ts[1].GetComponent<Image>().sprite = sprite;
		                # endif
		    		}
		    	}
			}
		}

		IEnumerator openCadFile(string cadName) {
			
			GameObject btnObject = GameObject.Find("Btn_" + cadName);
	        Transform[] transform = btnObject.GetComponentsInChildren<Transform>();

	        using (UnityWebRequest wr = UnityWebRequest.Get(baseUrl + cadName)) {
				wr.SendWebRequest();
				
				while (!wr.isDone) {
					transform[2].GetComponent<Text>().text = Math.Round(wr.downloadProgress * 100f, 2) + "%";
					yield return null;
			    }

			    if (wr.result == UnityWebRequest.Result.Success) {
					DriveFileData jsonFileData = JsonUtility.FromJson<DriveFileData>(wr.downloadHandler.text);
            		string fileURL = baseUrl + cadName + "?alt=media&token=" + jsonFileData.downloadTokens;

            		UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(fileURL);
    				www.SendWebRequest();

				    while (!www.isDone) {
				    	transform[2].GetComponent<Text>().text = Math.Round(www.downloadProgress * 100f, 2) + "%";
				    	yield return null;
				    }

			        if (www.result == UnityWebRequest.Result.Success) {

			        	GameObject.Find("EventSystem").SetActive(false);
						GameObject.Find("Canv_Options").SetActive(false);
						GameObject.Find("Canv_Main").SetActive(false);
						GameObject.Find("Camera").SetActive(false);

						Scene scene = SceneManager.CreateScene(cadName);
					  	SceneManager.SetActiveScene(scene);

					  	Material skyMaterial = Resources.Load("Skyboxes/Materials/CloudyMorningSky", typeof(Material)) as Material;
					  	RenderSettings.skybox = skyMaterial;
					  	
					  	GameObject lightGameObject = new GameObject("Sun");
				        Light lightComp = lightGameObject.AddComponent<Light>();
				        lightComp.type = LightType.Directional;
				        lightComp.transform.localPosition = new Vector3(13, 15, 0);
					  	lightComp.transform.localRotation = Quaternion.Euler(60, -50, 0);
						RenderSettings.sun = lightComp;

						GameObject controls = Resources.Load("Prefabs/Controls") as GameObject;
				        Transform[] ts = controls.GetComponentsInChildren<Transform>();
				        //PlayerArmature
				        ts[3].localPosition = new Vector3(0, 15, 0);

			        	GameObject asset = DownloadHandlerAssetBundle.GetContent(www).LoadAsset(cadName) as GameObject;

						if (asset.name == "DW06") {
							asset.transform.localPosition = new Vector3(17.35f, 0, 12);
				        } else if (asset.name == "EXPAN") {
				        	asset.transform.localPosition = new Vector3(37, 0, 0);
				        } else if (asset.name == "GE23") {
							asset.transform.localPosition = new Vector3(-3, 0, -1);
				        } else if (asset.name == "M2") {
							asset.transform.localPosition = new Vector3(-25, 0, -2);
				        } else if (asset.name == "RIVIERA") {
				        	asset.transform.localPosition = new Vector3(-12, 0, -4);
				        } else if (asset.name == "DestroyedCity") {
				        	ts[3].localPosition = new Vector3(0, 140, 0);
				        	asset.transform.localPosition = new Vector3(35, 0, 24);
				        } else if (asset.name == "Forest") {
				        	ts[3].localPosition = new Vector3(0, 50, 0);
				        	asset.transform.localPosition = new Vector3(185, 0, -50);
				        } else if (asset.name == "Mountains") {
				        	ts[3].localPosition = new Vector3(0, 150, 0);
				        	asset.transform.localPosition = new Vector3(-1740, 0, -1770);
				        }

		        		Instantiate(asset);
		        		Instantiate(controls);
	  				}
		        }		
			}

			yield return null;
        }

		[System.Serializable]
		public class Item {
	        public string name;
	        public string bucket;
	    }

	    [System.Serializable]
	    public class DriveFiles {
	        public List<string> prefixes;
	        public List<Item> items;
	    }

	    [System.Serializable]
	    public class DriveFileData {
		    public string name;
	        public string bucket;
	        public string generation;
	        public string metageneration;
	        public string contentType;
	        public string timeCreated;
	        public string updated;
	        public string storageClass;
	        public string size;
	        public string md5Hash;
	        public string contentEncoding;
	        public string contentDisposition;
	        public string crc32c;
	        public string etag;
	        public string downloadTokens;
	    }
	}
}