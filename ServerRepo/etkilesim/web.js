// JavaScript source code - manage request 
'use strict';
var express = require('express');
var crudl = require("../etkilesim/crudl");
var router = express.Router();

/* GET home page. */
router.get('/:field/:modeln/:type', function (req, res) { // list -> modeln,r_tuple, callback function whose inputs are success(int 0 or 1), model(s) found
    // if r_tuple={_id:2} then it act like "read"
    let field = req.params.field; // hangi modül? şuan tek modül var: monitoring: ./monitoring/datamodel.js
    let modeln = req.params.modeln; //hangi model? 
    let type = req.params.type; //ne tarz işlem?
    let r_tuple = req.query;
    console.dir(r_tuple);

    crudl.list(field, modeln, type, r_tuple, (s, m) => {

        if (s) {
            if (type == "c") {
                res.set('Content-Type', 'text/xml');
                res.send(m);
            }
            else {
                res.json(m);
            }
        }
        else {
            console.log("hata var gonderilemiyor 500");

            res.status(500);
            res.json(m);
        }
    }
    );
   // res.json({ "first": field, "modeln": modeln,"type":type });
});
router.post('/:field/:modeln/:type', function (req, res) { //create
    let field = req.params.field;
    let modeln = req.params.modeln;
    let type = req.params.type;
    let data = req.body;
    console.log("posted data:");
    console.dir(data);

    crudl.create(field, modeln, type, data, (s, m) => {

        if (s)
            res.json(m);
        else {
            
            res.status(500);
            res.json(m);
        }
    }
    );
});
router.delete('/:field/:modeln/:type', function (req, res) { //delete
    let field = req.params.field;
    let modeln = req.params.modeln;
    let type = req.params.type;
    let r_tuple = req.query;

    console.dir(r_tuple);

    crudl.delet(field, modeln, type, r_tuple, (s, m) => {

        if (s)
            res.json(m);
        else {
            res.status(500);
            res.json(m);
        }
    }
    );
});
router.put('/:field/:modeln/:type', function (req, res) { //update
    let field = req.params.field;
    let modeln = req.params.modeln;
    let type = req.params.type;
    let r_tuple = req.query;
    let update = req.body;

    console.dir(r_tuple);

    crudl.update(field, modeln, type, r_tuple, update, (s, m) => {

        if (s)
            res.json(m);
        else {
            res.status(500);
            res.json(m);
        }
    }
    );
});

module.exports = router;