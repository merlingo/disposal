 //veri modellerinin yonetimi icin kullanilmaktadir
var mongoose = require('mongoose')
    , Schema = mongoose.Schema;

// vulnerability'lerin cekildigi kaynaklar, cekme kodlari, input ve output yapisi, api-json istegi ile mi cekilir, craw mi edilir, calistirilmasi gereken komut ve beklenen outputda verinin parse edilmesi gerek mi,
//gerekiyorsa parse edilmesi icin kod parcasi
var siteSchema = new Schema({
    id: Number,
    link: String,
    input: [String], //api requests for 4 functions
    output: String, //expected output json type 
    type: String,//api, crawl or hardcopy
    command: String, // python code line to send request
    parseCode:String // if crawler needs, code for parsing response web site to get vulnerability list

});

//bulunan zaafiyetler veri tabanina bu model ile eklenir. cve, isim, etkilenen ürünler, cve_id
var vulnerabilitySchema = new Schema({
    id: Number,
    type: String,
    Source: String,
    cve: String,
    solution: String,
    rawData: String, // tablodakiler parse edilir. parse edilemeyen degerler rawData'ya konur ve vuln_attr ile eslestirilerek islenir.
    description: String,
    reference: String,
    rdate: { type: Date, default: Date.now },
        
});
//product ile vulnerability eşleştirmelerinin tutuldugu veri modeli
var provulnSchema = new Schema({
    cveid: String,
    product: String,
    version: String

});

//kullanicinin girdigi urunlerin listesi - urun adi, versiyon, son versiyon, web sitesi
var productSchema = new Schema({
    id: Number,
    vendor: String,
    name: String,
    latestVersion: String,
});
var client_product = new Schema({
    id: Number,
    name: String,
    version: String,
    clid: String
});

var attributeSchema = new Schema({
    value: String,
    inner: [this],
    desc: String,
    header: String, //farkli vuln tipleri icin ortak deger.
    type: String
});
var vuln_attr = new Schema({
    type: String,
    Source: String,// type bilgisinden hangi tür vulnerability'lerinin attribute oldugu
    attributes: [attributeSchema], // her event'in kendine has topladığı veri cinsleri vardır. Tümü rawData'da toplanır. rawData'nın parse edilmesi için attribute'ların bilinmesi gerekir. Oyüzden toplanan verinin attribute'ları bu listede string olarak tutulur.

});



var tagSchema = new Schema({
    id: Number,
    cveid:String,
    name: String,
    founded: String,

});

var exploitSchema = new Schema({
    id: Number,
    vulid: { type: Schema.Types.ObjectId, ref: 'Vulnerability' },
    name: String,
    rule: String,
    payload: String
});

var userSchema = new Schema({ //lisanslanan firmanın kullanicisi
    id: Number,
    name: String,
    mail: String,
    phone: String,
    password: String, //hashed value
    auth: String,
    clid:String,
});
//uye olan musterileri firma bilgisi - firma adi, sektor, kullandigi urunler, kullanicilar, 
var clientSchema = new Schema({ //lisans satılan firmaların bilgileri
    id: Number,
    company: String,
    lisenceType: String,
    lisenceYear: Number,
    //products: [productSchema],
    lbegdate: { type: Date, default: Date.now },
    lenddate: { type: Date, default: Date.now() + 365 * 24 * 60 *60000 },

});



var notificationSchema = new Schema({ //alarmlar
    id: Number,
    name: String,
    rule: String,
    product: String, // client product idsi
    clid:String,
    date: { type: Date, default: Date.now },

});

var suggestionsSchema = new Schema({ 
    suggestions : [String]
});
var vulnerabilityModel = mongoose.model('vulnerability', vulnerabilitySchema);
var siteModel = mongoose.model('site', siteSchema);
var attributeModel = mongoose.model('attribute', vuln_attr);
var productModel = mongoose.model('product', productSchema);
var client_productModel = mongoose.model('cproduct', client_product);

var tagModel = mongoose.model('tag', tagSchema);
var exploitModel = mongoose.model('exploit', exploitSchema);
var clientModel = mongoose.model('client', clientSchema);
var userModel = mongoose.model('user', userSchema);
var notificationModel = mongoose.model('notification', notificationSchema);
var suggestionsModel = mongoose.model('suggestion', suggestionsSchema);


//yeni bir product - vulnerability eslestirilmesi oldugunda bu fonksiyon calisir. 
provulnSchema.post('save', function(doc) {
    //bir urunde zaafiyet cikti - bu urune sahip firma var mı? varsa eger onlara alarm/bilgilendirme olusturulur.
    console.log('%s has been saved', JSON.stringify(doc));
    //if product in client_product, then create new notification
    client_productModel.find({ "name":doc.product }, (err, m) => {
        if (err) {
            console.log(err)
            //callback(0, err);
        } else {
            console.log('found model:  ', JSON.stringify(m));

            m.forEach(cprod => {
                var notification = new notificationModel({"name":doc.cveid,"rule":doc.cveid,"product":cprod.name,"clid":cprod.clid});
                notification.save(function (err, result) {
                    if (err) {
                        console.log("match post save: notification cannot be saved: "+JSON.stringify(err));

                    } else {
                        console.log("match post save: notification is saved: "+JSON.stringify(result));
                    }
                });
            });
            //callback(1, m);
        }
    });

  });
var matchesModel = mongoose.model('matches', provulnSchema);

module.exports.vulnerability = vulnerabilityModel;
module.exports.site = siteModel;
module.exports.attribute = attributeModel;
module.exports.product = productModel;
module.exports.cproduct = client_productModel;

module.exports.tag = tagModel;
module.exports.exploit = exploitModel;
module.exports.client = clientModel;
module.exports.user = userModel;
module.exports.notification = notificationModel;
module.exports.matches = matchesModel;

module.exports.getModel = function (modeln) {
    if (modeln == "vulnerability") {
        //istenen veri, türüne göre event_attr'si bulunur ve parse edilir.
        return vulnerabilityModel;
    }
    else if (modeln == "attribute") {
        //istenen veri, türüne göre event_attr'si bulunur ve parse edilir.
        return attributeModel;
    }
    else if (modeln == "product") {
        return productModel;
    }
    else if (modeln == "cproduct") {
        return client_productModel;
    }
    else if (modeln == "site") {
        return siteModel;
    } else if (modeln == "tag") {
        return tagModel;
    } else if (modeln == "exploit") {
        return exploitModel;
    } else if (modeln == "client") {
        return clientModel;
    } else if (modeln == "user") {
        return userModel;
    } else if (modeln == "notification") {
        return notificationModel;
    } else if (modeln == "suggestion") {
        return suggestionsModel;
    } else if (modeln == "match") {
        return matchesModel;
    }

}

