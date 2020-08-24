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

 
  //url:"http://localhost:50275/"

  // url:"http://192.168.0.105:9067/"
  url: "http://103.195.7.84:9092/",  
 //url:"http://192.168.0.105:8067/"

//  url: 'http://13.228.249.69:8060/',
// url: 'http://13.127.181.42:9096/',
  //LIVE
  // url:"https://www.kiposcollective.com:9095/"

   //StandBy
 // url:"https://www.kiposcollective.com:8065/"

};
