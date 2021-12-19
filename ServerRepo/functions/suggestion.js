var absRep = require('../DataService/AbsRepos');
 var monitoringService = new absRep(); 
 var Model = monitoringService.getModel("main", "suggestion");
 module.exports.newMethod1= function(inputs,callback){
     console.log(inputs);
     callback(0, "run func:newMethod");
};
