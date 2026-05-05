const path = require('path');
const SwaggerParser = require('@apidevtools/swagger-parser');

(async () => {
  const schemaPath = path.resolve(__dirname, '../../openapi/openapi.yaml');
  await SwaggerParser.validate(schemaPath);
  console.log('OpenAPI schema is valid:', schemaPath);
})().catch(err => {
  console.error('OpenAPI schema validation failed:', err.message);
  process.exit(1);
});
