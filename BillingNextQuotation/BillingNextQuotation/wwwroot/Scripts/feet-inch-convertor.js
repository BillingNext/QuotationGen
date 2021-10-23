
angular.module('feetAndInch', []).directive('inchesToFeetAndInches', [
    function () {
        return {
            restrict: 'E',
            templateUrl: '/StaticContent/feet-inches-form.html',
            controllerAs: 'ftIn',
            bindToController: true,
            scope: {
                measure: '='

            },
            controller: function () {
                this.displayMeasure = {};

                // Creating this reference to the controller since we will need
                // access to it in the displayMeasure property definitions and
                // at the same time need access to the displayMeasure object
                // itself.
                var ftIn = this;

                Object.defineProperties(this.displayMeasure, {
                    feet: {
                        get: function () {
                            return Math.floor(ftIn.measure / 12);
                        },
                        set: function (val) {
                            ftIn.measure = val * 12 + this.inches;
                        }
                    },
                    inches: {
                        get: function () {
                            return Math.floor(ftIn.measure % 12);
                        },
                        set: function (val) {
                            ftIn.measure = this.feet * 12 + val;
                        }
                    }
                });

                this.modelOptions = {
                    updateOn: 'default blur',
                    debounce: {
                        'default': 500,
                        blur: 0
                    }
                };
            }
        };
    }
])
    .controller('testApp', [
        function () {
            this.someMeasure = 0;
        }
    ]);