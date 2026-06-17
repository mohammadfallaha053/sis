const fs = require('fs');
const path = require('path');

const TEMPLATE_DIR = path.resolve(__dirname, '../.templates/FeatureTemplate'); // ← عدله إذا غيرت المجلد
const TARGET_BASE_DIR = path.resolve(__dirname, '../App');

function capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}
function lower(str) {
    return str.charAt(0).toLowerCase() + str.slice(1);
}
function upper(str) {
    return str.toUpperCase();
}

function generateFeature(featureName) {
    const Feature = capitalize(featureName);
    const feature = lower(featureName);
    const FEATURE = upper(featureName);

    function processDir(srcDir, destDir) {
        fs.mkdirSync(destDir, { recursive: true });

        for (const entry of fs.readdirSync(srcDir)) {
            const srcPath = path.join(srcDir, entry);
            const newEntry = entry
                .replaceAll('__Feature__', Feature)
                .replaceAll('__feature__', feature)
                .replaceAll('__FEATURE__', FEATURE);
            const destPath = path.join(destDir, newEntry);

            if (fs.lstatSync(srcPath).isDirectory()) {
                processDir(srcPath, destPath);
            } else {
                let content = fs.readFileSync(srcPath, 'utf8')
                    .replaceAll('__Feature__', Feature)
                    .replaceAll('__feature__', feature)
                    .replaceAll('__FEATURE__', FEATURE);
                fs.writeFileSync(destPath, content);
            }
        }
    }

    const targetPath = path.join(TARGET_BASE_DIR, Feature);
    if (fs.existsSync(targetPath)) {
        console.error(`❌ المجلد ${targetPath} موجود بالفعل.`);
        process.exit(1);
    }

    processDir(TEMPLATE_DIR, targetPath);
    console.log(`✅ تم إنشاء الفيتشر "${Feature}" في: App/${Feature}`);
}

const featureArg = process.argv[2];
if (!featureArg) {
    console.error('❌ يرجى تمرير اسم الفيتشر: node generate-feature.js Comments');
    process.exit(1);
}

generateFeature(featureArg);