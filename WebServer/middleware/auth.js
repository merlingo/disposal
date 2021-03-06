const jwt = require("jsonwebtoken");

const config = process.env;

const verifyToken = (req, res, next) => {
  const token =
    req.body.token || req.query.token || req.headers["x-access-token"];

  if (!token) {
    return res.status(403).send("A token is required for authentication");
  }
  try {
    console.log(config.TOKEN_KEY);
    const decoded = jwt.verify(token, "33743677397A244226452948404D6351");
    req.user = decoded;
    console.log( "from aut middleware req.user : "+req.user);
  } catch (err) {
    return res.status(401).send("Invalid Token");
  }
  return next();
};

module.exports = verifyToken;