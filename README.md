# Spaniel
Spaniel is a Xamarin.Forms app for iOS and Android to help run health checkups on your RESTful web service endpoints.  

Add a base URL for your project and then specify the endpoints to test. Run the endpoints to ensure they're up, healthy, and returning responses that you expect.


### Features
 - Standard layout for phone and master-detail layout for tablet devices
 - Quick actions for devices that support 3D Touch
 - See details from each HTTP response: status code, time, body
 - Run the endpoints all together, individually, or by search filter
 - Provide parameter values and filter definitions for each endpoint if desired
 - Ability to export a project as a .spaniel file and share via AirDrop, Email, or other apps and import into Spaniel on other devices
 - Handy swipe context actions to duplicate or delete endpoints, or edit a project's settings
 - Get up and running quickly! App comes pre-loaded with a project to test the HttpBin.org base URL and several of its endpoints as examples
 - Response bodies are cleaned up to save space whenever the app is backgrounded.


### Known Limitations 
 - Currently, only GET requests are supported - HttpClient.GetAsync()

#### iOS
 - Endpoint results from quick actions disappear after going back to ProjectList page and stay gone till app is force-closed
 - LaunchScreen.storyboard image on iPad doesn’t look too hot
 
#### Android
 - Styling needs a bit of love still
 - Need to test on a tablet device
 - Exporting a project only works with some apps (Google Drive)
 - Importing spaniel files doesn't work. For some reason, the OS doesn't recognize Spaniel as an app that can open .spaniel files.
 - Sometimes get this exception which causes an app crash, but don't know where it's coming from: Cannot access a disposed object. Object name: 'Xamarin.Forms.Platform.Android.TextCellRenderer+TextCellView'


## Build Status 
| iOS           | Android       |
| ------------- | ------------- |
| [![Build status](https://build.appcenter.ms/v0.1/apps/8edd5bde-f44e-4474-89d8-7c13fbb9f365/branches/master/badge)](https://appcenter.ms) | [![Build status](https://build.appcenter.ms/v0.1/apps/d6ce6820-4683-4d5f-ad51-2f2e633453ed/branches/master/badge)](https://appcenter.ms)  |

Thank you Visual Studio App Center!
