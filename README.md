# EliteFIPServer

Elite FIP Server is a .NET app which uses [EliteAPI](https://github.com/Somfic/EliteAPI) to 
read Elite Dangerous game information, and feed it to [Matric](https://matricapp.com) via the Matric 
Integration API. It also makes some game data available via a self-hosted web server, allowing the data 
to be viewed via browser or browser based viwer, and the display of that data to be customised.

The integration with Matric allows Matric to reflect the current game state in the UI, with particular respect to button/toggle state 
(such as Landing Gear or Lights), and also other information like current target data. Using a custom touch
screen display rather than a traditional keyboard increases immersion and lowers the requirement to remember
all the many key bindings you might need.

Current build is available [here](https://github.com/GeneralMADmanOne/EliteFIPServer/releases/)

If you are upgrading from a previous section, please double check the Runtime Pre-requisities and any upgrade notes as they might change from version to version.

---

## Runtime Pre-requisites
- Elite FIP Server is a .NET 6.0 application and requires the appropriate [runtime](https://dotnet.microsoft.com/download/dotnet/6.0/runtime) 
  to be installed.  
  
  For v2.x releases and later, you need the .NET Desktop Runtime (Run desktop apps). For v3.x releases and later, you **also** 
  need the ASP/NET Core Hosting Bundle (Run server apps).

- [Matric v2.x and the MatricIntegration.dll](https://matricapp.com)  
  Elite FIP Server supports integration with Matric v2.x via the MatricIntegration.dll provided in the Matric installation folder.
  This DLL **must** be copied to the Elite FIP Server folder or an exception will occur and the pre-packaged installation includes
  this file.
  

---

## Build Pre-Requisites
Aside from various libraries which VS will highlight if missing, and which are available via Nu Package Manager,
Elite FIP Server requires the following:

- [EliteAPI](https://github.com/Somfic/EliteAPI)
- [EliteFIPProtocol (my fork is needed for that)](https://github.com/GeneralMADmanOne/EliteFIPProtocol)
- [MatricIntegration.dll](https://matricapp.com)

Older versions of Elite FIP server, used the EliteJournalReader project to provide in-game events.

---

## Usage
Use at own risk :)
1. Build from source
2. Start Matric and connect a client.
3. Enable API Integration in Matric (Settings > API Integration > Enable 3rd party integration). Please note that PIN authorisation is no longer supported.
4. Run the EliteFIPServer.exe file
5. Matric Integration is not enabled by default. You can start this manually from the UI, and configure it to start automatically in the Settings tab. 
6. The Panel Server (which pubishes game data via a built-in  Web Server) can also be started manually, and configured to start automatically in the Settings tab.

### Matric Authorisation
Elite FIP Server v2 does not support Matric PIN authorisation. Please disable this in Matric.

### Matric Clients
Elite FIP Server no longer restricts users to a single Matric Client device - all connected Matric clients 
will receive game state updates

### Enable Logging
In the 'Settings' panel the Enable Logging option will enable or disable logging. By default logging is turned 
off. When enabled, the log is located in the User AppData\Roaming\EliteFIPServer folder.
For example: c:\Users\MyUserName\AppData\Roaming\EliteFIPServer

### Autostart Matric Integration
**Not recommended** The Settings tab allows Matric Integration to be enabled when Elite FIP server starts. This should only be enabled if you always start Matric before
starting Elite FIP Server. See Known issues for further information.

### Autostart Panel Server
The Settings tab allows the Panel Server  to be enabled when Elite FIP server starts. See [Panel server](EliteFIPServer/PanelServer.md) for more information

### Enable Custom Button Text
To have Elite FIP server change button text when game state changes, you have to update the ButtonTextConfig.json file in the same folder
where the Elite FIP Server is run from. A sample file is provided with all currently customisable buttons (the button names match those
described in the current feature section below). You should not change the button name, only the Off/On text for those buttons you want to customise,
and the flag to enable the update for that button.

For example, to have Elite FIP Server change the button state for the HudMode button, edit the following line:
```
{"ButtonName": "HudMode", "OffText":"Hud Mode", "OnText":"Hud Mode", "UpdateButtonText": false},
```
to something like 
```
{"ButtonName": "HudMode", "OffText":"Combat", "OnText":"Analysis", "UpdateButtonText": true},
```
Save the file and either restart Elite FIP Server or go to the Settings panel and click save without changing 
any settings (which triggers a reload of the config file).


---

## Known Issues

 - You must start Matric before starting Matric Integration in Elite FIP Server (either manually or automatically). If you start 
   Matric Integration in Elite FIP Server before starting Matric, the Integration capability will fail and cannot be started unless
   you restart Elite FIP Server. It is therefore not recommended to enable Autostart of Matric integration at this time.

 - When you arrive at the final destination system of a planned route, the route information will disappear during the final hyerspace
   jump. This is working as intended, and is due to the sequence of events emitted by Elite Dangerous. Essentially Elite clears the route
   automatically during the last jump, before the arrival event in the final system, resulting in the route being cleared in FIP server.
   To mitigate this, Elite FIP server provides a 'Previous Route' which will show the completed route if desired. See 
   [Panel server](EliteFIPServer/PanelServer.md) for more information

 - Fuel Gauges may not always work properly after you have changed Ship, normally it should fix itself after first Refuel.
   

---

## Upgrading from v1.x

- Elite FIP Server v2.x and later include changes that will cause some Matric decks/existing buttons to not function as expected.
When upgrading from v1, please review the button name information below and change your buttons in Matric accordingly.

---

# Current Features
EliteFIPServer should work with any Deck (a sample deck is available [here](https://community.matricapp.com/deck/435/ed-fip-test)), and will update Matric objects 
based on the Matric button name assigned. To use with your own deck, set the Name field of the Matric control to the listed below as appropriate, including the prefix to 
indicate the type of button, and that control will be updated based on Elite state. Examples are below.

### Button Types
Elite FIP Server can provide several different types of button update for the same data source, to allow flexibility in designing your own deck. The button type will determine
how Elite FIP Server changes the matric component, and is defined by a three letter prefix of the Name:

#### Indicator (ind)
Changes Matric button state to on or off depending on the state in Elite. This is what might be considered a typical or standard Matric button and should be used when you need a simple toggle button (like Landing Gear), or a current state (like Mass Locked).

#### Warning (wrn)
Sets the Matric button state to off while the corresponding game state is 'off'. When the game state is 'on', the Matric button state will be toggled on and off to provide 
a flashing effect. This should be used when you need a more visible alert of current state (for example, Overheat).  

#### Button (btn)
By default, this is the same as Indicator. For Button, you can enable an additional function which will also change the text of the button as well as the state, based on the current
game state. For example. when switching between Combat and Analysis HUD modes, the button text can be set to change to show "Combat" or "Analysis" to indicate the current mode. 
See Usage Instructions for how to enable this.

#### Switch (swt) 
For use with multi-position switches. All simple toggle controls support use of a 2-way multiposition switch, with position 1 being off and 2 being on.
Specific controls supporting more than 2-way switches might be added later - if you have a specific use case, please contact the developer.

#### Slider (sld) 
Used to create Guages. Specific values will depend on the button.

#### Text (txt)
A flat text field, used for lables and text information like target data. These are special cases and information is provided below.

### Button Support Matrix

Elite Data Point | Base Matric Button Name | Indicator | Warning | Button | Switch 
-------------- | ----------- | ----------- | -------------- | ----------- | ----------- 
Docked (at a station) | Docked | x | x | | 
Landed (on a planet) | Landed | x | x | | 
Landing Gear   | LandingGear | x | x | x | x  
Shields Down* | Shields | x | x | | 
Supercruise*   | Supercruise | x | x | x | x  
Flight Assist *  | FlightAssist | x | x | x | x  
Hardpoints     | Hardpoints | x | x | x | x  
In wing | InWing | x | x | | 
Ship Lights    | Lights | x | x | x | x  
Cargo Scoop    | CargoScoop | x | x | x | x  
Silent Running | SilentRunning | x | x | x | x  
Scooping fuel | ScoopingFuel | x | x | | 
SRV Handbrake | SrvHandbrake | x | x | x | x  
SRV Turret | SrvTurret | x | x | x | x  
SRV Under Ship | SrvUnderShip|  x | x | |
SRV Drive Assist | SrvDriveAssist | x | x | x | x  
Mass locked | FSDMassLock | x | x | | 
FSD Charging | FSDCharging | x | x | | 
FSD Cooldown | FSDCooldown | x | x | | 
Low fuel | LowFuel | x | x | | 
Overheat | Overheat | x | x | | 
In danger | InDanger | x | x | | 
Being interdicted | Interdiction | x | x | | 
In Main Ship | InMainShip | x | x | | 
In Fighter | InFighter | x | x | | 
In SRV | InSRV | x | x | | 
HUD Mode | HudMode | x | x | x | x  
Night Vision  | NightVision | x | x | x | x  
FSD Jump*     | FsdJump | x | x | x | x  
SRV High Beam | SrvHighBeam | x | x | x | x  
 | | | | |   
On Foot | OnFoot | x | x | | 
In Taxi | InTaxi | x | x | | 
In Multicrew | InMulticrew | x | x | | 
On Foot (Station) | OnFootInStation | x | x | | 
On Foot (Planet) | OnFootOnPlanet | x | x | | 
Aim Down Sight | AimDownSight | x | x | x | x 
Low Oxygen | LowOxygen | x | x | | 
Low Health | LowHealth | x | x | | 
Cold | Cold | x | x | | 
Hot | Hot | x | x | | 
Very Cold | VeryCold | x | x | | 
Very Hot | VeryHot | x | x | | 


\* Unlike the other indicators Shields and Flight Assist are typically 'active' and you want to be warned/informed if they are not. 
In these cases, the default state is 'on' and the state will change and warn if they are off. See the example deck for ideas on how this can be handled gracefully in Matric.

\* Supercruise will only illuminate when actually in Supercruise (after charging and the initial short FSD jump to get there)

\* FSD Jump indicator is also used when entering Supercruise, so will illuminate briefly at that point.

#### Examples
To indicate if Landing Gear is deployed or not, set the Name field for the control in the Matric editor to: indLandingGear
To have a flashing warning when Landing gear is deployed, set the Name field for the control in the Matric editor to: wrnLandingGear
When using a 2-way Multi-position switch for Landing gear, set the Name field for the control in the Matric editor to: swtLandingGear

### Sliders and Text Fields

Elite Data Point | Base Matric Button Name | Slider | Text | Notes
-------------- | ----------- | ----------- | -------------- | -----------
Fuel (Main) | FuelMain | x | x | Slider is a percentage, text is actual value from game.
Fuel (Reservoir) | FuelReservoir | x | x | Slider is a percentage, text is actual value from game. 
Target data (Ship type, Faction, Rank etc)   | Target |  | x | Custom panel showing target information in one Matric button
Target labels  | TargetLabel |  | x | Fixed label for Target panel
Status data (Legal state, Cargo weight, Fuel etc)   | Status  |  | x | Custom panel showing status information in one Matric button
Status labels  | StatusLabel|  | x | Fixed label for Status panel
Target data (Ship type, Shield/Hull/SubSys data, Rank, Faction, Legal state, Bounty etc)   | Target  |  | x | Custom panel showing status information in one Matric button
Target labels  | TargetLabel|  | x | Fixed label for Status panel
Infos  (Closest Body, Legal State, Destination Name, Main Fuel, Fuel Reservoir, Cargo, Balance)  | Info  |  | x | Custom panel showing information in one Matric button
Info labels  | InfoLabel|  | x | Fixed label for Info panel
Landing data (Station Name, System Name, Body Name, Distance Main Star, Economy, Landing Pad, Deny Reason, Cargo, Balance) depending on Docking Event  | Landing  |  | x | Custom panel showing status information in one Matric button
Landing labels  | LandingLabel|  | x | Fixed label for Status panel
Landing Pad Number  | Landingpad|  | x | Fixed label for Landing Pad Number
Target Subsys Name  | TargetSubsysName|  | x | Fixed label for Target Subsystem Name
Target Shield Value | TargetShieldValue | x | x | Slider is a percentage, text is actual value from game. 
Target Hull Value | TargetHullValue | x | x | Slider is a percentage, text is actual value from game. 
Target Subsys Value | TargetSubsysValue | x | x | Slider is a percentage, text is actual value from game. 
Game Infos (Fid, Commander, has Horizons/Odyssey, GameMode, Language, GameVersion, Build, Group) | GameInfo  |  | x | Custom panel showing Game information in one Matric button



Text displays require a text button, of sufficient width and height to display the full text. If the text button
is not large enough for the content, the behaviour is 'undefined'.
Text size is per standard Matric setting, but for text which combines multiple Elite data points in one display,  eeach field is defined on a new line. 


---
# Change History

### v3.3.0
- My initial version (based on 3.2.0 from EarthstormSoftware)
- Added new Status & Info Panels
- Fixed Fuel Sliders for Main and Reservoir Fuel (not perfect but works most of the time)
- Added Sliders/Text Fields for Target Shiels/Hull/Subsystem
- Added Landing Pad Number Test field and Landing Info Panel


find older version history [here](https://github.com/EarthstormSoftware/EliteFIPServer)

---
# Thanks to...
- EarthstormSoftware for providing this great software, I just enhanced it a bit
- Somfic and all the contributers to EliteAPI
- AnarchyZG - the developer of Matric, both for the software and the support
- The developers and contributors to EliteJournalReader
- The developers and contributors to all the other open tools and packages that made this feasible. 




