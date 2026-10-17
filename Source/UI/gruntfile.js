const path = require('node:path');
const { fileURLToPath } = require('node:url');
const sass = require('sass');
const implementation = Object.assign({}, sass, {
    render: (options, callback) => {
        sass.render(options, (error, result) => {
            if (error) {
                callback(error);
                return;
            }

            const sourceMap = JSON.parse(result.map.toString());
            sourceMap.sources = sourceMap.sources.map(source =>
                source.startsWith('file://')
                    ? path.relative(path.dirname(options.outFile), fileURLToPath(source)).split(path.sep).join('/')
                    : source);
            sourceMap.sourceRoot = '';

            callback(null, Object.assign({}, result, {
                css: Buffer.from(result.css.toString().replace(/\r?\n/g, '\r\n')),
                map: Buffer.from(JSON.stringify(sourceMap))
            }));
        });
    }
});

module.exports = function (grunt) {
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        sass: {
            options: {
                implementation: implementation,
                linefeed: 'crlf',
                sourceMap: true
            },
            dist: {
                files: {
                    'css/bootstrap.css': 'sass/imports/bootstrap.scss',
                    'css/fonts.css': 'sass/imports/fonts.scss',
                    'css/theme.css': 'sass/imports/theme.scss',
                    'css/nemestats.css': 'sass/imports/nemestats.scss'
                }
            }
        },
        watch: {
            css: {
                files: 'sass/**/*.scss',
                tasks: ['sass']
            }
        },
        copy: {
            dist: {
                files: [{
                    //for font-awesome
                    expand: true,
                    flatten: true,
                    cwd: 'node_modules/font-awesome',
                    src: ['fonts/*.*'],
                    dest: 'fonts/'
                }]
            },
        }
    });
    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.loadNpmTasks("grunt-sass");
    grunt.loadNpmTasks("grunt-contrib-copy");

    grunt.registerTask('default', ['sass','copy','watch']);
};
