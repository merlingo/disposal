'use strict';
var express = require('express');
const axios = require('axios');
const jwt = require("jsonwebtoken");
const bcrypt = require("bcryptjs");

var sendRequest = require("./sendRequest");
const auth = require("../middleware/auth");

var router = express.Router();
//GET RESTFUL API REUEST FROM REACT-UI, send request to repository, get and prepare data, send response to ui
/*Yardımcı */
// create new alarm rule, run rule, list alarm by date, change status of alarm, get one alarm details, delete rule, delete alarm
function generalSendRequestPattern(path, res_func) {
    const options = {
        host: 'localhost',
        port: 5000,
        path: path,
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    };
    sendRequest.getJSON(options, (status, obj) => {
        res_func(obj);
    });
}

function columnEkle(mdbtable, label, field) {
    mdbtable["columns"].push({ "label": label, "field": field, "sort": "asc", "width": 140 });
    return mdbtable;
}

//yardımcı fonksiyon - suggestions ve vulnerabilities icin tags, products ve vulnerability.description icinde arama yapilir. bulunanlar ayni listeye koyulur.
function produceSuggestions(sugg,callback) {
    //sugg kelimesi icinde & ya da | or ile ayrılır. and ya da or olmasına gore bu kelimeler birlikte ya da ayrı ayrı tags, products ve vulnerability.cve ya da vulnerability.description icinde aranır. 

    var vul_filter ="rel=or&"+ "cve="+sugg+"&desc="+sugg;//cve ya da description'da ara
    var product_filter = "rel=or&" + "name="+sugg+"&vendor="+sugg;
    var tag_filter = "rel=or&" + "name="+sugg;


    let one = "http://localhost:5000/main/vulnerability/m?" + vul_filter
    let two = "http://localhost:5000/main/product/m?" + product_filter
    let three = "http://localhost:5000/main/tag/m?" + tag_filter

    const requestOne = axios.get(one);
    const requestTwo = axios.get(two);
    const requestThree = axios.get(three);


    axios.all([requestOne, requestTwo, requestThree]).then(axios.spread((...responses) => {
        const response_vul = responses[0] //get vulnerabilities related with model
        const response_pro = responses[1]
        const respones_tag = responses[2]
        console.log(response_vul.data);

        var data = response_vul.data;
        //console.log(data);
        //data = data.concat(response_pro.data.concat(respones_tag.data));

        callback(data);
        // use/access the results 
    })).catch(errors => {
        // react on errors.
        console.log(errors);
        callback(errors);
    })

}
function findVulnerabilities(keyword, callback) {
    //keyword ile vulnerability aramasi ve tag product fonksiyonlarinin cagrimlari yapilir.

    var vul_filter = "rel=or&" + "cve=" + keyword + "&desc=" + keyword;//cve ya da description'da ara


    let one = "http://localhost:5000/main/vulnerability/m?" + vul_filter
    let two = 'http://localhost:5000/main/match/f?fname=vulnerabilityFromProduct&inputs=["' + keyword + '"]';
    let three = 'http://localhost:5000/main/tag/f?fname=vulnerabilityFromTag&inputs=["' + keyword + '"]'

    const requestOne = axios.get(one);
    const requestTwo = axios.get(two);
    const requestThree = axios.get(three);


    axios.all([requestOne, requestTwo, requestThree]).then(axios.spread((...responses) => {
        const response_vul = responses[0]; //get vulnerabilities related with model
        const response_pro = responses[1];
        const respones_tag = responses[2];
        console.log("vulnerebility results:");
        console.log(response_vul.data);
        console.log("product results");
        console.log(response_pro.data);
        console.log("respones_tag results");
        console.log(respones_tag.data);
        var data = response_vul.data;
        data = data.concat(response_pro.data.concat(respones_tag.data));

        callback(data);
        // use/access the results 
    })).catch(errors => {
        // react on errors.
        console.log(errors);
        callback(errors);
    })

}
/* Web Icin servisler*/

router.get('/products',auth, function (req, res) {
    var url = "";
    axios.get(url)
        .then((res) => {
            //ürünler res modelinde alınır. tüm productlar array icinde bunun icindedir. 
        });
});

//
router.get('/vulnerabilities', function (req, res) { //nested request example
    var v = req.query.v;
    console.log(v);
    findVulnerabilities(v, data => {
        res.json(data);

    });
});



/*PROFIL */
//hesap olustur, hesap sil, profil görüntüle, login, logout eventalarm
router.get('/tags',auth, function (req, res) {
    res.send('respond with a resource');
});

/*SENSOR */
//yeni sensor olustur, sensor bilgilerini getir, sensor bilgilerini değiştir, sensore fonksiyon ekle sil, sensor durum bilgisi, sensoru yeniden başlat, sensoru güncelle
router.get('/sensor',auth, function (req, res) {



});

router.post('/sensor/',auth, function (req, res) { //post data example
    
});

const getUser = async function(umail) {
    const res = await axios('http://localhost:5000/main/user/m?view=last&mail='+umail);
    console.log(res);
    return await res.data;
  }

router.post('/login/', async function (req, res) { //post data example
    
    console.log(req.body);
    
    // Our login logic starts here
  try {
    // Get user input
    const { email, password } = req.body;
    //var cred = JSON.stringify(req.body);

    // Validate user input
    if (!(email && password)) {
      res.status(400).send("All input is required");
    }
    // Validate if user exist in our database
    const user = await getUser(email);
    console.log(JSON.stringify(user));
    if (user ) {
        if((await bcrypt.compare(password, user.password)))
        {
            console.log("hash uyustu!!\n"+ process.env.TOKEN_KEY);
            //console.log(process.env);
            // Create token
            const token = jwt.sign(
                { clid: user.clid, email },
                "33743677397A244226452948404D6351",//process.env.TOKEN_KEY,
                {
                expiresIn: "2h",
                }
            );

            // save user token
            user.token = token;
            user.message = "successful";
            // user
            res.status(200).json(user);
        }
//        res.status(400).send("Invalid Credentials");
        else
            res.status(400).send("bad password");
    }
    else{
  //        res.status(400).send("Invalid Credentials");

        res.status(400).send("no email");

    }
  } catch (err) {
    console.log(err);
    res.status(500).send("bad try: \n"+err);

  }

    
    //res.json({message:"ok",token:"xxx"});


});

router.get('/vulnerability/',auth, function (req, res) { //get vulnerability list attached with their tags
    
    var cred = JSON.stringify(req.body);
    console.log(req.body);
    let vul_string = "http://localhost:5000/main/vulnerability/m?";

    axios.get(vul_string).then((repores)=>{
        console.log("vulnerebility results:");
        console.log(repores.data);
        var data = repores.data;
        res.json(data);

    }).catch(errors => {
        // react on errors.
        console.log(errors);
        res.json(errors);
    });
});
router.get('/product/',auth, function (req, res) { //get vulnerability list attached with their tags
    
    console.log(req.body);
    let vul_string = "http://localhost:5000/main/product/m?";

    axios.get(vul_string).then((repores)=>{
        console.log("product results:");
        console.log(repores.data);
        var data = repores.data;
        res.json(data);

    }).catch(errors => {
        // react on errors.
        console.log(errors);
        res.json(errors);
    });
});
router.post('/product/:clid',auth, function (req, res) { //get vulnerability list attached with their tags
    
    console.log(req.body);
    console.log(req.params.clid);

    const client_product = {name:req.body.name, clid:req.params.clid};
    let vul_string = "http://localhost:5000/main/cproduct/m?";

    axios.post(vul_string, client_product ).then((repores)=>{
        console.log("products of client results:");
        console.log(repores.data);
        var data = repores.data;
        res.json(data);

    }).catch(errors => {
        // react on errors.
        console.log(errors);
        res.json(errors);
    });
});
router.delete('/product/:clid',auth, function (req, res) { //get vulnerability list attached with their tags
    
    console.log(req.body);
    console.log(req.params.clid);

    const client_product = {name:req.body.name, clid:req.params.clid};
    let vul_string = "http://localhost:5000/main/cproduct/m?";

    axios.delete(vul_string, client_product ).then((repores)=>{
        console.log("products of client results:");
        console.log(repores.data);
        var data = repores.data;
        res.json(data);

    }).catch(errors => {
        // react on errors.
        console.log(errors);
        res.json(errors);
    });
});
router.get('/product/vuls',auth, function (req, res) { //get vulnerability list attached with their tags
    
    console.log(req.body);
    var p = req.query.p;
    console.log(req.query);

    let vul_string = 'http://localhost:5000/main/match/f?fname=vulnerabilityFromProduct&inputs=["' + p + '"]';;

    axios.get(vul_string).then((repores)=>{
        console.log("vulnerabiilties of product results:");
        console.log(repores.data);
        var data = repores.data;
        res.json(data);

    }).catch(errors => {
        // react on errors.
        console.log(errors);
        res.json(errors);
    });
});
router.get('/alarm/',auth, function (req, res) { //get vulnerability list attached with their tags
    
    console.log(req.body);
    let vul_string = "http://localhost:5000/main/notification/m?";

    axios.get(vul_string).then((repores)=>{
        console.log("alarms results:");
        console.log(repores.data);
        var data = repores.data;
        res.json(data);

    }).catch(errors => {
        // react on errors.
        console.log(errors);
        res.json(errors);
    });
});
router.get('/alarm/options',auth, function (req, res) { //get vulnerability list attached with their tags
    
    console.log(req.body);
    let vul_string = "http://localhost:5000/main/option/m?";

    axios.get(vul_string).then((repores)=>{
        console.log("option results:");
        console.log(repores.data);
        var data = repores.data;
        res.json(data);

    }).catch(errors => {
        // react on errors.
        console.log(errors);
        res.json(errors);
    });
});
router.post('/alarm/options/:clid',auth, function (req, res) { //get vulnerability list attached with their tags
    
    console.log(req.body);
    console.log(req.params.clid);

    const option = {...req.body, clid:req.params.clid};
    let req_string = "http://localhost:5000/main/option/m?";

    axios.post(req_string, option ).then((repores)=>{
        console.log("options of client results:");
        console.log(repores.data);
        var data = repores.data;
        res.json(data);

    }).catch(errors => {
        // react on errors.
        console.log(errors);
        res.json(errors);
    });
});
module.exports = router;