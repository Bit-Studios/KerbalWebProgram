using System.Collections;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using ShadowUtilityLIB;
using Shapes;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Color = UnityEngine.Color;
using Logger = ShadowUtilityLIB.logging.Logger;
using MouseButton = PuppeteerSharp.Input.MouseButton;

namespace KerbalWebProgram.UI
{
	public class Browser 
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public int ScreenX { get; set; }
		public int ScreenY { get; set; }
		private static Logger logger = new Logger(KWPmod.ModName, KWPmod.ModVersion);
		private Thread browserThread;
		private IBrowser browser;
		private IPage page;
		private string Screenshot = "";

		private Texture2D BrowserScreenshot;
		public UIDocument window;

		public bool HasKeyInput = false;

		public List<KeyCode> SpecialKeys = new List<KeyCode>() { KeyCode.LeftControl,KeyCode.Backspace,KeyCode.RightControl };
		public Browser(string title, string url, int width, int height,int screenx, int screeny) { 
			Title = title;
			Url = url;
			Width = width;
			Height = height;
			ScreenX = screenx;
			ScreenY = screeny;
			try
			{
				VisualElement BrowserMenu = Element.Root(Title);
			   
				BrowserMenu.style.position = Position.Absolute;
				BrowserMenu.style.width = Width;
				BrowserMenu.style.height = Height;
				BrowserMenu.style.left = ScreenX;
				BrowserMenu.style.top = ScreenY;
				BrowserMenu.style.marginBottom = 0;
				BrowserMenu.style.marginRight = 0;
				BrowserMenu.style.marginLeft = 0;
				BrowserMenu.style.marginTop = 0;
				BrowserMenu.style.paddingBottom = 0;
				BrowserMenu.style.paddingLeft = 0;
				BrowserMenu.style.paddingRight = 0;
				BrowserMenu.style.paddingTop = 0;
				BrowserMenu.focusable = true;

				

				VisualElement ToolBar = Element.VisualElement($"{Title}-ToolBar");
				ToolBar.style.width = width;
				ToolBar.style.height = 15;
				ToolBar.style.top = -15;
				ToolBar.style.backgroundColor = Color.black;
				ToolBar.style.marginBottom = 0;
				ToolBar.style.marginRight = 0;
				ToolBar.style.marginLeft = 0;
				ToolBar.style.marginTop = 0;
				ToolBar.style.paddingBottom = 0;
				ToolBar.style.paddingLeft = 0;
				ToolBar.style.paddingRight = 0;
				ToolBar.style.paddingTop = 0;
				BrowserMenu.Add(ToolBar);

				Button ToolBarButtonClose = Element.Button($"{Title}-ToolBarButtonClose", $"X");
				ToolBarButtonClose.style.position = Position.Absolute;
				ToolBarButtonClose.style.width = 10;
				ToolBarButtonClose.style.height = 10;
				ToolBarButtonClose.style.paddingTop = 2;
				ToolBarButtonClose.style.paddingBottom = 2;
				ToolBarButtonClose.style.paddingLeft = 2;
				ToolBarButtonClose.style.paddingRight = 2;
				ToolBarButtonClose.style.top = 3.5f;
				ToolBarButtonClose.style.right = 10;
				ToolBarButtonClose.style.fontSize = 12;
				ToolBarButtonClose.style.color = Color.white;
				ToolBarButtonClose.style.backgroundColor = Color.red;
				ToolBarButtonClose.style.borderRightColor = new StyleColor(new Color32(255, 255, 255, 0));
				ToolBarButtonClose.style.borderLeftColor = new StyleColor(new Color32(255, 255, 255, 0));
				ToolBarButtonClose.style.borderTopColor = new StyleColor(new Color32(255, 255, 255, 0));
				ToolBarButtonClose.style.borderBottomColor = new StyleColor(new Color32(255, 255, 255, 0));
				
                ToolBarButtonClose.clickable = new Clickable(() => {
					Close();
				});
				ToolBar.Add(ToolBarButtonClose);

				Label MessageLabel = Element.Label($"{Title}-MessageLabel", $"Loading");
                MessageLabel.style.position = Position.Absolute;
                MessageLabel.style.width = width;
                MessageLabel.style.height = height;
                MessageLabel.style.paddingTop = 0;
                MessageLabel.style.paddingBottom = 0;
                MessageLabel.style.paddingLeft = 0;
                MessageLabel.style.paddingRight = 0;
				MessageLabel.style.top = 0;
				MessageLabel.style.right = 0;
				MessageLabel.style.fontSize = 30;
				MessageLabel.style.color = Color.white;
				MessageLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
				
				MessageLabel.style.borderRightColor = new StyleColor(new Color32(255, 255, 255, 0));
				MessageLabel.style.borderLeftColor = new StyleColor(new Color32(255, 255, 255, 0));
				MessageLabel.style.borderTopColor = new StyleColor(new Color32(255, 255, 255, 0));
				MessageLabel.style.borderBottomColor = new StyleColor(new Color32(255, 255, 255, 0));
				BrowserMenu.Add(MessageLabel);

				//Label ToolBarLabelKeyboard = Element.Label($"{Title}-ToolBarLabelKeyboard", $"Input");
				//ToolBarLabelKeyboard.style.position = Position.Absolute;
				//ToolBarLabelKeyboard.style.width = 40;
				//ToolBarLabelKeyboard.style.height = 15;
				//ToolBarLabelKeyboard.style.paddingTop = 0;
				//ToolBarLabelKeyboard.style.paddingBottom = 0;
				//ToolBarLabelKeyboard.style.paddingLeft = 0;
				//ToolBarLabelKeyboard.style.paddingRight = 0;
				//ToolBarLabelKeyboard.style.top = 3.5f;
				//ToolBarLabelKeyboard.style.right = 55;
				//ToolBarLabelKeyboard.style.fontSize = 10;
				//ToolBarLabelKeyboard.style.color = Color.white;
				//ToolBarLabelKeyboard.style.backgroundColor = Color.red;
				//ToolBarLabelKeyboard.style.borderRightColor = new StyleColor(new Color32(255, 255, 255, 0));
				//ToolBarLabelKeyboard.style.borderLeftColor = new StyleColor(new Color32(255, 255, 255, 0));
				//ToolBarLabelKeyboard.style.borderTopColor = new StyleColor(new Color32(255, 255, 255, 0));
				//ToolBarLabelKeyboard.style.borderBottomColor = new StyleColor(new Color32(255, 255, 255, 0));
				//ToolBar.Add(ToolBarLabelKeyboard);
				BrowserMenu.RegisterCallback<KeyDownEvent>((evt) => {
					try
					{
						logger.Log($"key {evt.character}");
						
						if (SpecialKeys.Contains(evt.keyCode))
						{
							page.Keyboard.PressAsync(evt.keyCode.ToString());
						}
						else
						{
							page.Keyboard.PressAsync(evt.character.ToString());
						}
					}
					catch (Exception e)
					{
						logger.Error($"{e}\n{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}\n{e.GetBaseException()}");
					}
				});

				BrowserMenu.RegisterCallback<ClickEvent>((evt) =>
				{
					try
					{
						BrowserMenu.Focus();
						//logger.Log($"Mouse button {evt.button} position G {evt.position} L {evt.localPosition}");
						switch (evt.button)
						{
							case 0:
								page.Mouse.ClickAsync((decimal)evt.localPosition.x, (decimal)evt.localPosition.y, new ClickOptions { Button = MouseButton.Left });
								break;
							case 1:
								page.Mouse.ClickAsync((decimal)evt.localPosition.x, (decimal)evt.localPosition.y, new ClickOptions { Button = MouseButton.Right });
								break;
							case 2:
								page.Mouse.ClickAsync((decimal)evt.localPosition.x, (decimal)evt.localPosition.y, new ClickOptions { Button = MouseButton.Middle });
								break;
						}
					}
					catch (Exception e)
					{
						logger.Error($"{e}\n{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}\n{e.GetBaseException()}");
					}
				});
				BrowserMenu.RegisterCallback<WheelEvent>((evt) =>
				{
					logger.Log($"Mouse delta {evt.delta} mousePosition {evt.mousePosition}");
					page.Mouse.WheelAsync((decimal)evt.delta.x * 100, (decimal)evt.delta.y * 100);
				});
				window = Window.CreateFromElement(BrowserMenu);
				PanelSettings panelSettings = new PanelSettings();
				panelSettings.themeUss = window.panelSettings.themeUss;
				panelSettings.themeStyleSheet = window.panelSettings.themeStyleSheet;
				window.panelSettings = panelSettings;
				BrowserScreenshot = new Texture2D(width, height);
			}
			catch (Exception e)
			{
				logger.Error($"{e}\n{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}\n{e.GetBaseException()}");
			}
			new Thread(new ThreadStart(() =>
			{
				Task.Run(async () => await GenerateBrowserInstanceAsync());

			})).Start();
			ShadowLIB.RunCr(UpdateWindow());
		}
		public IEnumerator UpdateWindow()
		{
			new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					try
					{
						Thread.Sleep(16);
						var taskwait = page.ScreenshotBase64Async();
						Screenshot = taskwait.Result;
                        window.rootVisualElement.Q<Label>($"{Title}-MessageLabel").visible = false;
                    }
					catch (Exception e)
					{
						window.rootVisualElement.Q<Label>($"{Title}-MessageLabel").visible = true;
                        //logger.Error($"{e}\n{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}\n{e.GetBaseException()}");
                    }
				}
			})).Start();
			//yield return new WaitForSeconds(5);
			while (true)
			{
				yield return new WaitForSeconds(0.016f);
				try
				{
					BrowserScreenshot.LoadImage(Convert.FromBase64String(Screenshot));
					window.rootVisualElement.Q(Title).style.backgroundImage = BrowserScreenshot;
				}
				catch (Exception e)
				{
					//logger.Error($"{e}\n{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}\n{e.GetBaseException()}");
				}
			}
		}
		public async Task GenerateBrowserInstanceAsync()
		{
			
			try
			{

				using var browserFetcher = new BrowserFetcher();
				await browserFetcher.DownloadAsync();
				browser = await Puppeteer.LaunchAsync(new LaunchOptions
				{
					IgnoredDefaultArgs = new[] { "--mute-audio" },
					Headless = true
				}); 
				
				page = await browser.NewPageAsync();
				await page.SetViewportAsync(new ViewPortOptions
				{
					Width = Width ,
					Height = Height ,
					DeviceScaleFactor = 1
				});
				
				await page.GoToAsync(Url);
				string Screenshot = await page.ScreenshotBase64Async();
				//BrowserScreenshot = new Texture2D(Width, Height);

			}
			catch (Exception e)
			{
				logger.Error($"{e}\n{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}\n{e.GetBaseException()}");
			}

		}
		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			var brcl = browser.CloseAsync();
			browser.Dispose();
			window.rootVisualElement.Children().ForEach((ve) =>
			{
				ve.name = Guid.NewGuid().ToString();
			});
			window.rootVisualElement.style.width = 0;
			window.rootVisualElement.style.height = 0;
			window.rootVisualElement.visible = false;
			window.rootVisualElement.name = Guid.NewGuid().ToString();
			window.gameObject.SetActive(false);
			window.gameObject.DestroyGameObject();
		}
	}
}
