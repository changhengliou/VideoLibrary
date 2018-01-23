const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const autoprefixer = require('autoprefixer');

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    const inputPath = path.join(__dirname, 'ClientApp', 'app.js');
    const outputPath = path.join(__dirname, '/Static', '/dist');
    const devServerPath = path.join(__dirname);

    return {
        stats: { modules: false },
        resolve: { extensions: ['.js', '.jsx'], alias: { moment: 'moment/moment.js' } },
        entry: {
            site: inputPath, // custom js share by all pages
            vendor: ['jquery', 'bootstrap', 'superagent', 'webfontloader'], // external js share by all pages
            resource: path.join(__dirname, 'ClientApp', 'resource.js'), // individual js used in each page
            signin: path.join(__dirname, 'ClientApp', 'signin.js'),
            location: path.join(__dirname, 'ClientApp', 'location.js'),
            sosetting: path.join(__dirname, 'ClientApp', 'soSetting.js'),
            events: path.join(__dirname, 'ClientApp', 'events.js'),
        },
        output: {
            filename: '[name].js',
            path: outputPath,
            publicPath: '/dist',
        },
        devServer:
            {
                index: 'index.html',
                contentBase: devServerPath,
                hot: true,
                historyApiFallback: true,
                host: "0.0.0.0",
                port: 8888
            },
        module: {
            rules: [
                {
                    test: require.resolve('jquery'),
                    use: [
                        { loader: 'expose-loader', options: 'jQuery' },
                        { loader: 'expose-loader', options: '$' }
                    ]
                },
                {
                    test: /\.js(x?)$/,
                    include: /ClientApp/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            cacheDirectory: isDevBuild,
                            babelrc: false,
                            presets: [
                                [
                                    'env', {
                                        targets: { 'browsers': ['last 2 versions', 'ie >= 7'] },
                                        modules: false,
                                        useBuiltIns: false,
                                        debug: false,
                                    }
                                ],
                                'react', 'stage-0'
                            ],
                            plugins: ['transform-runtime'].concat(
                                isDevBuild ? [] :
                                    [
                                        'transform-react-constant-elements',
                                        'transform-react-inline-elements'
                                    ])
                        }
                    }
                },
                {
                    test: /\.css$/,
                    use: ExtractTextPlugin.extract({
                        use: [
                            {
                                loader: 'css-loader',
                                options: {
                                    module: false,
                                    minimize: isDevBuild ? false : true,
                                    localIdentName: '_[name]__[local]_[hash:base64:5]'
                                }
                            },
                            {
                                loader: 'postcss-loader',
                                options: { plugins: () => [autoprefixer] }
                            }
                        ]
                    })
                },
                {
                    test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/,
                    use: 'url-loader?limit=100000'
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery' }),
            new webpack.ContextReplacementPlugin(/moment[\/\\]locale$/, /zh-tw/),
            new ExtractTextPlugin('[name].css'),
            new webpack.optimize.CommonsChunkPlugin(
                { name: 'vendor', fileName: 'vendor.js', minChunks: Infinity }),
        ]
            .concat(
            isDevBuild ?
                [
                    new webpack.HotModuleReplacementPlugin(),
                    new webpack.SourceMapDevToolPlugin({
                        filename: '[file].map',
                        moduleFilenameTemplate: path.relative(
                            outputPath,
                            '[resourcePath]')  // Point sourcemap entries
                        // to the original file
                        // locations on disk
                    })
                ] :
                [new webpack.optimize.UglifyJsPlugin()])
    };
};