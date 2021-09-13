import { promisify } from 'util';
import { basename, extname, dirname, join as joinPath } from 'path';
import { promises as fs } from 'fs';
import { parseMarbleDiagramSpecification } from '@swirly/parser';
import { lightStyles } from '@swirly/theme-default-light';
import { renderMarbleDiagram } from '@swirly/renderer-node';
import globWithCallback from 'glob';

const glob = promisify(globWithCallback);

const inputFiles = await glob("src/**/*.swirly");

await Promise.all(inputFiles.map(async path => {
    const svgFileName = basename(path, extname(path)) + ".svg";
    const svgFilePath = joinPath(dirname(path), svgFileName);

    const unparsedSpec = await fs.readFile(path, 'utf8');
    const spec = await parseMarbleDiagramSpecification(unparsedSpec);
    const { xml } = renderMarbleDiagram(spec, { lightStyles });
    await fs.writeFile(svgFilePath, xml, { encoding: 'utf8' });

    console.log(`${path} -> ${svgFilePath}`);
}));
