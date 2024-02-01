/// <binding BeforeBuild='default' Clean='clean' />

// post-install.js

/**
 * Script to run after npm install
 *
 * Copy selected files to user's directory
 */

var path = require('node:path')
var fs = require('node:fs')
var vendors = require('./vendor.json')

// User's local directory
const userPath = process.env.INIT_CWD
console.log(userPath)

const paths = {
    nodeModulesBase: 'node_modules',
    destinationBase: 'wwwroot/vendor'
}

fs.rmSync(path.join(userPath, paths.destinationBase), { force: true, recursive: true })

// Copying files to user's local directory
vendors.forEach((filePath) => {    
    let destPath = filePath.replace(paths.nodeModulesBase, paths.destinationBase)
    destPath = path.join(userPath, destPath)
    console.log('Copy:' + filePath + ' to ' + destPath)
    fs.cpSync(filePath, destPath)
})

process.exit()

