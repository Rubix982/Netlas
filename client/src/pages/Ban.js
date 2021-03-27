import { Helmet } from 'react-helmet';
import { Box, Container } from '@material-ui/core';
import BanListResults from 'src/components/ban/BanListResults';
import BanListToolbar from 'src/components/ban/BanListToolbar';
import customers from 'src/__mocks__/customers';

const BanList = () => (
  <>
    <Helmet>
      <title>Bans</title>
    </Helmet>
    <Box
      sx={{
        backgroundColor: 'background.default',
        minHeight: '100%',
        py: 3
      }}
    >
      <Container maxWidth={false}>
        <BanListToolbar />
        <Box sx={{ pt: 3 }}>
          <BanListResults customers={customers} />
        </Box>
      </Container>
    </Box>
  </>
);

export default BanList;
