//dinamik olarak veri modelleri icin fonksiyon ekleme, cikarma ve silme islemlerinden sorumlu modul
//
//exports:
//Add new function (model, functionName, func_payload): eklenen model icin dosya yoksa olusturulur. dosyaya fonksiyon kalıba uygun şekilde eklenir. fonksiyon bilgileri veritabanina kayit edilir.
//execute function (model, functionName, inputs, callback): ilgili modelin dosyasi eklenir. dosyadan fonksiyon çekilir. inputlarla fonksiyon calıstırılır ve sonuc dondurulur ya da callback fonksiyon çalıştırılır.
//remove function (model, functionName): 
const fs = require('fs');
var Path = require('path');
var totality = require("../DataService/totalitymodel");

function createfile(filename,modeln, payload) {
    var p = 'var absRep = require(\'../DataService/AbsRepos\');\n var monitoringService = new absRep(); \n var Model = monitoringService.getModel("main", "' + modeln + '");'+payload;
    fs.writeFile(filename, p, function (err) {
        if (err) throw err;
        console.log('Saved!');
    });
}

 function isExist(modeln, funcname, callback) {
    var Model = totality.getModel("function");
    // const query = Model.find({ model: modeln, name: funcname });
     Model.find({ model: modeln, name: funcname }).exec().then(m => {
         console.log(m);
         callback(m.length > 0);
     });
   
   
}
function insertDB(modeln, funcname, inputs, output, func_payload, aftersave) {
    const time = Date.now();
    var Model = totality.getModel("function");
    var data = { model: modeln, time: time, name: funcname, inputType: inputs, output: output, func_payload: func_payload }; //TODO: type degeri alinmadi ve girilmedi. Her fonksiyonun işlev türü olur.veritabanında bu belirtilebilir.
    var model = new Model(data);
    model.save(function (err, result) {
        if (err) {
            aftersave(0, err);
        } else {
            aftersave(1, result);
        }
    });
}
function saveFunctionCall(model, func, inputs, output, callback, aftersave) {
    var Model = totality.getModel("functionCall");
    var data = { model: model, name: func, input: inputs, output: output, callback: callback.toString() }; //TODO: type degeri alinmadi ve girilmedi. Her fonksiyonun işlev türü olur.veritabanında bu belirtilebilir.
    var model = new Model(data);
    model.save(function (err, result) {
        if (err) {
            aftersave(0, err);
        } else {
            aftersave(1, result);
        }
    });
}
function get_model(modelName){
    return require("./"+modelName);
}

function run_func(model, func, input, callback) {

    //function is exist?
    isExist(model, func, (e) => {
        if (!e) {
            var error = new Error("CANNOT RUN FUNCTION - IT DOESNT EXIST " + model + " " + func);
            callback(error);
            return;
        }

        var m = get_model(model);

        saveFunctionCall(model, func, input, "", callback, function (s, d) {
            if (s > 0) {
                console.log("FUNCTION CALL: veritabanina kayit edildi:");
               // console.log(d);
            }
            else {
                console.log("FUNCTION CALL: veritabanina kayit edilirken hata cikti:");
                console.log(d);
            }
            m[func](input, callback);
            return;
        });
        return
    });
}

module.exports.execute = run_func;

module.exports.add = function (functname, filename, func_payload, callback) {

     isExist(filename, functname, (e) => {
         if (e) {
             var error = new Error("FUNCTION IS EXIST IN DB" + filename + " " + functname);
             callback(error);
             return;
         }
         else {

             //console.log(__dirname);
             const path = Path.join(__dirname, filename + ".js");
             console.log(path);

             var func_temp = "\n module.exports." + functname + "= function(inputs,callback){\n" + func_payload + "\n};\n";

             if (!fs.existsSync(path)) {
                 createfile(path, filename, func_temp);
                 insertDB(filename, functname, [], "", func_payload, function (s, d) {
                     if (s > 0) {
                         console.log("FUNCTION : veritabanina kayit edildi:");
                         console.log(d);
                     }
                     else {
                         console.log("FUNCTION CALL: veritabanina kayit edilirken hata cikti:");
                         console.log(d);
                     }
                     callback(0, d);
                     return;
                 });
                 console.log('function model file is inserted');
                 return;
             }
             else {
                 fs.appendFile(path, func_temp, (err) => {
                     if (err) {
                         console.log("cant append file");
                         console.log(err);
                         callback(err);
                        return;
                     }
                     console.log('function model file is uploaded');
                     insertDB(filename, functname, [], "", func_payload, function (s, d) {
                         if (s > 0) {
                             console.log("FUNCTION : veritabanina kayit edildi:");
                             console.log(d);
                         }
                         else {
                             console.log("FUNCTION CALL: veritabanina kayit edilirken hata cikti:");
                             console.log(d);
                         }
                         callback(0, d);
                         return;
                     });
                 });
             }
         }
         return;
    });
    

    return;
}   
//TODO: fonksiyonlarin veritabanina eklenmesi - veritabanina ekleme ve orada var mi diye kontrol etme. eger veritabaninda varsa fonksiyon o fonksiyon tekrar eklenmez.

//TODO: fonksiyon silme ve update