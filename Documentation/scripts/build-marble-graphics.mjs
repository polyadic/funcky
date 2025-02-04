import { promisify } from 'util';
import { basename, extname, dirname, join as joinPath } from 'path';
import { promises as fs } from 'fs';
import { parseMarbleDiagramSpecification } from '@swirly/parser';
import { lightStyles } from '@swirly/theme-default-light';
import { darkStyles } from '@swirly/theme-default-dark';
import { renderMarbleDiagram } from '@swirly/renderer-node';
import globWithCallback from 'glob';

const glob = promisify(globWithCallback);

const inputFiles = await glob("src/**/*.swirly");

await Promise.all(inputFiles.map(async path => {
    const svgFileName = basename(path, extname(path)) + ".svg";
    const darkSvgFileName = basename(path, extname(path)) + "-dark.svg"
    const svgFilePath = joinPath(dirname(path), svgFileName);
    const darkSvgPath = joinPath(dirname(path), darkSvgFileName);

    const unparsedSpec = await fs.readFile(path, 'utf8');
    const spec = await parseMarbleDiagramSpecification(unparsedSpec);
    const { xml } = renderMarbleDiagram(spec, { styles: lightStyles });
    await fs.writeFile(svgFilePath, xml, { encoding: 'utf8' });
    const { xml: darkXml } = renderMarbleDiagram(spec, { styles: darkStyles });
    await fs.writeFile(darkSvgPath, darkXml, { encoding: 'utf8' });

    console.log(`${path} -> ${svgFilePath}`);
}));
