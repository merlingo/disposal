var absRep = require('../DataService/AbsRepos');
 var monitoringService = new absRep(); 
 var Model = monitoringService.getModel("main", "tag");
 module.exports.vulnerabilityFromTag= function(inputs,callback){
    console.log(inputs);
    var tagvul = [];
     for (p in inputs) {
         tagvul.push({ 'name': inputs[p] });
     }
     Model.find({ $or:tagvul},function(err,tagvuln){
         if (!err) {
             var tagModel = monitoringService.getModel("main", "vulnerability");
            var vulns = [];
             for (p in tagvuln) {
                 vulns.push({ 'cve': tagvuln[p].cveid });
             }
             tagModel.find({ $or: vulns }, function (e, vulnerabilities) {
                 if (!e) callback(0, vulnerabilities); else callback(e);
             });
         }
         else callback(err);
     });
};
