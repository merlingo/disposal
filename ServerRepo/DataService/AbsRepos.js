// Abstract Repository Class - it is used for the module who manage data -> keep, organize, integrate, divide,
//veritabanına bağlanma fonksiyonu abstract fonksiyondur
//eğer strict ise mongoose schema'ları döndüren fonksiyonu olacaktır. model ismini alacak ve onun schemasını döndürecek. 
//eğer strict değil ise sadece veritabanını paylaşır. oradan CRUDL çalıştırılır.


//Veri yönetimi yapan modül (Repository) 3 değer üstlenir: Field,model,type
//1.	Field ile hangi bölüm olduğu girilir.
//2.	 Model, alan içinde o alanın bir parçası olan modele işaret eder.
//3.	Type, isteğin türüdür.Belli başlı istek tipleri vardır: manipulation -> veri ekleme silme, function -> modelin herhangi bir fonksiyonunu çağırma, ekleme ya da silme, configuration -> modele yeni ayar ekleme, değiştirme silme ya da isteme.

// this is our abstract class
var main = require("./datamodel");
var manmodel = require("./manmodel");

var AbstractRepository = function () {
    if (this.constructor === AbstractRepository) {
        throw new Error('Cannot instanciate abstract class');
    }
};

// these are our abstract method -  module have to implement these - field model ve type girdilerini alır bir aksiyon çalıştırır ve sonucu döner. bu fonksiyon bu işlevi görmek için kullanılır.
AbstractRepository.prototype.getModel = function (field, modeln, type) {
    throw new Error('Cannot call abstract method get model - bu arayüzü kullanmak için lütfen getModel fonksiyonunu implement ediniz')
    return model;
};
AbstractRepository.prototype.connectDB = function (connstr) {
    throw new Error('Cannot call abstract method connect DB - bu arayüzü kullanmak için lütfen connectDB fonksiyonunu implement ediniz')
}
class monitorRepository extends AbstractRepository {
    //her repository module,  node paketi olarak yazılır ve projeye öyle eklenir. sunucu olarak config dosyası üstünden DataService arayüzünün app.js'si çalıştırılır
    constructor() {
        super();
    }
     getModel(field, modeln) {
         var f;
         console.dir("field:" + field);
        if (field === "main") {//main package which consists of data models
            f = main;
        }
        else if (field === "admin") {
            f = manmodel;
        }
        else {
            throw new Error("FIELD ADI DOĞRU SEÇİLMELİDİR");
        }
         //field = monitoring/datamodel
         console.log("field:" + field + " model:" + modeln);
         var m = f.getModel(modeln); //event
         console.dir(modeln);
         return m;

    }
    getSchema(field, modeln) {
        var f;
        console.dir("field:" + field);
        if (field === "main") {//main package which consists of data models
            f = main;
        }
        else if (field === "admin") {
            f = manmodel;
        }
        else {
            throw new Error("FIELD ADI DOĞRU SEÇİLMELİDİR");
        }
        //field = monitoring/datamodel
        console.log("field:" + field + " model:" + modeln);
        var s = f.getSchema(modeln); //event
        console.dir("schema - "+modeln);
        return s;

    }
     connectDB(connstr) {
        var mongoose = require("mongoose");
        //var connection_string = ""; //'mongodb://localhost:27017/myapp'
        mongoose.connect(connstr, { useNewUrlParser: true });
    }
}
module.exports = monitorRepository;
