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
                    'css/theme.css': 'sass/imports/theme.scss',
                    'css/nemestats.css': 'sass/imports/nemestats.scss'
                }
            }
        },
        watch: {
            css: {
                files: 'sass/**/*.scss',
                tasks: ['sass_globbing','sass']
            }
        }
    });
    grunt.loadNpmTasks('grunt-sass-globbing');
    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.loadNpmTasks("grunt-sass");

    grunt.registerTask('default', ['watch']);
};