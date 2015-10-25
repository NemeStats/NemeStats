module.exports = function(grunt) {
    grunt.initConfig({
        sass: {
            dist: {
                files: [
                    {
                        expand: true,
                        src: ["*.scss"],
                        dest: "css",
                        ext: ".min.css"
                    }
                ]
            }
        },
        watch: {
            sass: {
                files: ["**/*.scss"],
                tasks: ["sass"],
                options: {
                    livereload: true
                }
            }
        }
    });

    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.loadNpmTasks("grunt-contrib-sass");

    grunt.registerTask('default', ['sass', 'watch']);
};