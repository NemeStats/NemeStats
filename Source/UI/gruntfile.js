module.exports = function (grunt) {
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        sass: {
            options: {
                sourceMap: true
            },
            dist: {
                files: {
                    'css/bootstrap.css': 'sass/imports/bootstrap.scss',
                    'css/fontawesome.css': 'sass/imports/fontawesome.scss',
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
    grunt.loadNpmTasks('grunt-sass-globbing');
    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.loadNpmTasks("grunt-sass");
    grunt.loadNpmTasks("grunt-contrib-copy");

    grunt.registerTask('default', ['copy','watch']);
};