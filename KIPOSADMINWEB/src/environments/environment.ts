// import { Level } from 'angular2-logger/core';
//import { NGXLogger } from 'ngx-logger';

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
    appVersion: "0.5.0",
    appLanguage: "en",
    channel: "Admin",
    debug: {
        alertForGlobalExceptionErrors: false,
        formatJSONStringify: true,
        offlineApp: false
    },
    maxSessionTimeoutPreLogin : 1800,
    maxSessionTimeoutPostLogin : 1800,
    idleTimeoutPreLogin: 1800,
    idleTimeoutPostLogin: 1800,
    itemsByPage: 15,
    // logger: {
    //     level: NGXLogger.LOG,
    //     enabled: false
    // },
    production: true,
    requestTimeout: 120,
    secureExt: "",
    serviceExt: "",
    // secureExt: "/MiddlewareRestServices/SecureComm",
    // serviceExt: "/MiddlewareRestServices/BusinessServiceServlet",
    systemMaintenance: {
        plannedMaintenance: false,
        unplannedMaintenance: false
    },

    //apiURL: 'http://localhost:15536/',
  // url:"http://localhost:50275/"

// local QA 192.168.0.26
  // apiURL:"http://192.168.0.26:9066/",
  // url: "http://192.168.0.26:9067/",
  
  // urlLink :'http://192.168.0.26:9064/#/',
  
  
     //live QA Training Urls
 //  apiURL: 'http://103.195.7.84:8084/',
 //  url: 'http://103.195.7.84:8082/',
 //  urlLink :'http://103.195.7.84:8081/',
  
  // Live QA 103.195.7.84
  
  //  apiURL:"http://103.195.7.84:9094/",
  //  url: "http://103.195.7.84:9092/",  
  //  urlLink :'http://103.195.7.84:9091/',
	
	


  // apiURL:"http://192.168.0.105:7066/", 
  // url: "http://192.168.0.105:8067/"


  // apiURL: 'http://13.228.249.69:9092/',
  // url: 'http://13.228.249.69:8060/',
// QA
//  apiURL: 'http://13.127.181.42:9099/',
//   url: 'http://13.127.181.42:9096/',


   
	 //LIVE
     apiURL:'https://www.store-kiposcollective.com:9096/',
      url:'https://www.store-kiposcollective.com:9443/',
       urlLink:'https://www.store-kiposcollective.com/#/'

   //StandBy
  // apiURL:"https://www.kiposcollective.com:9096/", 
  // url:"https://www.kiposcollective.com:8065/"
};
