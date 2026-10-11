import { SimNaoPipe } from './sim-nao.pipe';

// BibliotecaRHC.Web/tsconfig.spec.json
{
  "extends": "./tsconfig.json",
  "compilerOptions": {
    "outDir": "./out-tsc/spec",
    "types": ["jasmine", "node"]
  },
  "files": [
    "src/test.ts",
    "src/polyfills.ts"
  ],
  "include": [
    "src/**/*.spec.ts",
    "src/**/*.d.ts"
  ]
}describe('SimNaoPipe', () => {
  it('create an instance', () => {
    const pipe = new SimNaoPipe();
    expect(pipe).toBeTruthy();
  });
});
