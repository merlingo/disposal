var absRep = require("../DataService/AbsRepos");
var func_man = require("../functions/function_management");
var xmlTojs = require("xml-js");
monitoringService = new absRep();
// JavaScript source code - Bu arayüzün etkileşim fonksiyonlarının sunulduğu fonksiyonlar - CRUDL fonksiyonları
var crudl = {
     create : function (field, modeln, type, data, aftersave) {
         var Model = monitoringService.getModel(field, modeln);
         if (type == "m") {
             //model'e yeni veri eklenir
             var model = new Model(data);//model is gotten from abstract Repository class
             model.save(function (err, result) {
                 if (err) {
                     aftersave(0, err);
                 } else {
                     aftersave(1, result);
                 }
             });
         }
         else if (type == "f") {
             //functions klasorune eğer yoksa model eklenir. dosyanın en altına fonksiyon eklenir. fonksiyon ekleme pattern'ı vardır o sekilde eklenir. dosyada eklenen fonksiyon export edilmistir.
             // fonksiyon belirtilen isimle kullanilir. 
             var fname = data.fname;
             var func_payload = data.func_payload;
             console.log("crudl.create.f");
             console.log(data);


                 func_man.add(fname, modeln, func_payload, function (err, result) {
                     if (err) {
                         aftersave(0, err);
                     } else {
                         aftersave(1, result);
                     }
                 });
         }
         else if (type == "c") {
             //model için yeni config alanı ya da dosyası oluşturulur
         }

},
    list: function (field, modeln, type, r_tuple, callback) {// if r_tuple={_id:2} then it act like "read"
        var Model = monitoringService.getModel(field, modeln);
        console.dir("type:"+type);
        if (type == "m") {
            //model veritabanından çekilir
            console.log("r_tuple:" + JSON.stringify(r_tuple));
            if ((r_tuple) && (r_tuple["view"] == "last")) {
                delete r_tuple["view"];
                Model.findOne(r_tuple, {}, { sort: { 'date': -1 } }, (err, m) => {
                    if (err) {
                        callback(0, err);
                    } else {
                        callback(1, m);
                    }
                });
            }
            else {
                if ((r_tuple) && (r_tuple["rel"] == "or")) {
                    delete r_tuple["rel"];
                    //special modified with OR QUERY:
                    var query_arr = Object.keys(r_tuple).map((key) => { var r = {}; r[key] = { $regex: r_tuple[key] }; return r; });
                    console.log(query_arr)

                    Model.find({ $or: query_arr }, (err, m) => {
                        if (err) {
                            console.log(err)
                            callback(0, err);
                        } else {
                            callback(1, m);
                        }
                    });
                }
                else {
                    console.log("empty query get all"+JSON.stringify(Model));

                    Model.find(r_tuple, (err, m) => {
                        if (err) {
                            console.log(err)
                            callback(0, err);
                        } else {
                            console.log("bulundu donuyor. ilki:"+JSON.stringify(m[0]))
                            callback(1, m);
                        }
                    });
                }
                
            }

        }
         else if(type == "f") {
            //model fonksiyonu çalıştırılır ya da r_tuple'a bağlı olarak tüm fonksiyonlar listelenir.
            var fname = r_tuple["fname"];
            console.log(fname);
            var inputs = JSON.parse(r_tuple["inputs"]);
            console.log(inputs);

            func_man.execute(modeln, fname, inputs, callback);
        }
         else if (type == "c") {
            //model config dosyası xml olarak döndürülür - istenilen model xml dosyasina dönüştürülür
            Model.find(r_tuple, (err, m) => {
                var result = xmlTojs.json2xml(m, { compact: true, spaces: 4 });
                if (err) {
                    callback(0, xmlTojs.js2xml(err));
                } else {
                    callback(1, result);
                }
            });
        }

    },

    update: function (field, modeln, type, filter, update, callback) {

        var Model = monitoringService.getModel(field, modeln);
        if (type == "m") {
            //model güncellenir
             Model.findOneAndUpdate(filter,  update, { upsert: true, new:true }, (err, m) => {
                 if (err) {
                     callback(0, err);
                 } else {
                     callback(1, m);
                 }
                });
        }
        else if (type == "f") {
            //modelin fonksiyonu güncellenir
        }
        else if (type == "c") {
            //modelin config verileri güncellenir
        }
    },
     delet : function (field, modeln, type, r_tuple, callback) {
         var Model = monitoringService.getModel(field, modeln);
         if (type == "m") {
             //model silinir
             Model.deleteMany(r_tuple, function (err,m) {
                 if (err) {
                     callback(0, err);
                 } else {
                     var result = {}
                     result["result"] = "success"
                     result["count"]=m.length
                     callback(1, result);
                 }
                 });
         }
         else if (type == "f") {
             //model fonksiyonu silinir
         }
         else if (type == "c") {
             //modelin config verileri silinir
         }

    }
}
module.exports = crudl;