﻿using EliteFIPProtocol;
using EliteFIPServer.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace EliteFIPServer
{
    public class PanelServer {

        CoreServer serverCore;

        public ComponentState CurrentState { get; private set; }
        Task PanelServerTask;
        private CancellationTokenSource PanelServerCTS;
        GameDataUpdateController GameDataUpdateController;        

        public PanelServer(CoreServer serverCore) {
            this.serverCore = serverCore;
            this.CurrentState = new ComponentState();
            ClientConnect.SetDataProvider(serverCore.EliteAPIIntegration);
        }

        public void Start() {
            Log.Instance.Info("Panel Server starting");
            CurrentState.Set(RunState.Starting);
            bool panelServerStarted = false;
            

            try {
                var panelServerUrl = "http://*:" + Properties.Settings.Default.PanelServerPort;
                var panelServerBuilder = WebApplication.CreateBuilder(EliteFIPServerApplication.GetArgs());

                panelServerBuilder.Services.AddMvcCore().AddMvcOptions(options => options.EnableEndpointRouting=false);
                panelServerBuilder.Services.AddCors(cors => cors.AddPolicy("CorsPolicy", builder => {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithOrigins(panelServerUrl);
                }));
                panelServerBuilder.Services.AddControllers().AddNewtonsoftJson();
                panelServerBuilder.Services.AddSignalR();
                var panelServer = panelServerBuilder.Build();

                if (panelServer.Environment.IsDevelopment()) {
                    panelServer.UseDeveloperExceptionPage();
                }

                Log.Instance.Info("Listening on {panelserverurl}", panelServerUrl);
                panelServer.Urls.Add(panelServerUrl);
                panelServer.UseStaticFiles();
                panelServer.UseRouting();
                panelServer.UseMvc();
                panelServer.UseCors("CorsPolicy");

                panelServer.MapHub<GameDataUpdateHub>("/gamedataupdatehub");
                var hubContext = panelServer.Services.GetService(typeof(IHubContext<GameDataUpdateHub>)) as IHubContext<GameDataUpdateHub>;
                GameDataUpdateController = new GameDataUpdateController(hubContext);


                PanelServerCTS = new CancellationTokenSource();
                PanelServerTask = panelServer.RunAsync(PanelServerCTS.Token);
                PanelServerTask.ContinueWith(PanelServerThreadEnded);
                panelServerStarted = true;
            } catch (Exception ex) {
                Log.Instance.Error("Exception: {exception}", ex.ToString());
                panelServerStarted = false;
            }

            CurrentState.Set(panelServerStarted ? RunState.Started : RunState.Stopped);            
            Log.Instance.Info("Panel server start complete");
        }

        public void Stop() {
            Log.Instance.Info("Panel server stopping");            
            // Stop Panel Server
            if (CurrentState.State == RunState.Started) {
                CurrentState.Set(RunState.Stopping);
                PanelServerCTS.Cancel();
            }            
        }

        private void PanelServerThreadEnded(Task task) {
            if (task.Exception != null) {
                Log.Instance.Info("Panel Server Thread Exception: {exception}", task.Exception.ToString());
            }
            CurrentState.Set(RunState.Stopped);
            Log.Instance.Info("Panel Server Thread ended");
        }

        public void UpdateGameState(GameEventType eventType, Object gameData) {

            // Only attempt to publish if Panel Server is running
            if (CurrentState.State == RunState.Started) {

                if (eventType == GameEventType.Status) {
                    StatusData currentStatus = gameData as StatusData;
                    GameDataUpdateController.SendStatusUpdate(currentStatus);

                } else if (eventType == GameEventType.Target) {
                    ShipTargetedData currentTarget = gameData as ShipTargetedData;                    
                    GameDataUpdateController.SendTargetUpdate(currentTarget); 

                } else if (eventType == GameEventType.Location) {
                    LocationData currentLocation = gameData as LocationData;
                    GameDataUpdateController.SendLocationUpdate(currentLocation);

                } else if (eventType == GameEventType.Navigation) {
                    NavigationData currentNavRoute = gameData as NavigationData;                    
                    GameDataUpdateController.SendNavRouteUpdate(currentNavRoute);

                } else if (eventType == GameEventType.PreviousNavRoute) {
                    NavigationData previousNavRoute = gameData as NavigationData;
                    GameDataUpdateController.SendPreviousNavRoute(previousNavRoute);

                } else if (eventType == GameEventType.Jump) {
                    JumpData currentJumpData = gameData as JumpData;
                    GameDataUpdateController.SendJumpUpdate(currentJumpData);

                } else if (eventType == GameEventType.DockingGranted) {
                    DockingGrantedData currentDockingGranted = gameData as DockingGrantedData;
                    GameDataUpdateController.SendDockingGrantedUpdate(currentDockingGranted);

                }
                else if (eventType == GameEventType.DockingDenied)
                {
                    DockingDeniedData currentDockingDenied = gameData as DockingDeniedData;
                    GameDataUpdateController.SendDockingDeniedUpdate(currentDockingDenied);

                }
                else if (eventType == GameEventType.DockingTimeout)
                {
                    DockingTimeoutData currentDockingTimeout = gameData as DockingTimeoutData;
                    GameDataUpdateController.SendDockingTimeoutUpdate(currentDockingTimeout);

                }
                else if (eventType == GameEventType.DockingCancelled)
                {
                    DockingCancelledData currentDockingCancelled = gameData as DockingCancelledData;
                    GameDataUpdateController.SendDockingCancelledUpdate(currentDockingCancelled);
                }
                else if (eventType == GameEventType.RefuelAll)
                {
                    RefuelAllData currentRefuelAll = gameData as RefuelAllData;
                    GameDataUpdateController.SendRefuelAllUpdate(currentRefuelAll);
                }
                else if (eventType == GameEventType.RefuelPartial)
                {
                    RefuelPartialData currentRefuelPartial = gameData as RefuelPartialData;
                    GameDataUpdateController.SendRefuelPartialUpdate(currentRefuelPartial);
                }
                else if (eventType == GameEventType.ReservoirReplenished)
                {
                    ReservoirReplenishedData currentReservoirReplenished = gameData as ReservoirReplenishedData;
                    GameDataUpdateController.SendReservoirReplenishedUpdate(currentReservoirReplenished);
                }
                else if (eventType == GameEventType.LoadGame)
                {
                    LoadGameData currentLoadGame = gameData as LoadGameData;
                    GameDataUpdateController.SendLoadGameUpdate(currentLoadGame);
                }
                else if (eventType == GameEventType.Loadout)
                {
                    LoadoutData currentLoadout = gameData as LoadoutData;
                    GameDataUpdateController.SendLoadoutUpdate(currentLoadout);
                }
                else if (eventType == GameEventType.ShipyardNew)
                {
                    ShipyardNewData currentShipyardNew = gameData as ShipyardNewData;
                    GameDataUpdateController.SendShipyardNewUpdate(currentShipyardNew);
                }
                else if (eventType == GameEventType.ShipyardSwap)
                {
                    ShipyardSwapData currentShipyardSwap = gameData as ShipyardSwapData;
                    GameDataUpdateController.SendShipyardSwapUpdate(currentShipyardSwap);
                }
            }
        }
    }
}
