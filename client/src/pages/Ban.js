import React from 'react';
import { Helmet } from 'react-helmet';
import { Box, Container } from '@material-ui/core';
import BanListResults from 'src/components/ban/BanListResults';
import BanListToolbar from 'src/components/ban/BanListToolbar';

// API
import API from 'src/API/API';

const BanList = () => {
  const [dataReady, setDataReady] = React.useState(false);
  const finalObjectsToSend = [];

  React.useEffect(async () => {
    const response = await API.getRequest('https://localhost/forbidden');

    response.domains.forEach((item) => {
      finalObjectsToSend.push({
        name: '-',
        email: '-',
        domainName: item,
        ip: '-',
        addedDate: Date(),
      });
    });
    setDataReady(true);
  });

  return (
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
            {dataReady && (<BanListResults customers={finalObjectsToSend} />)}
          </Box>
        </Container>
      </Box>
    </>
  );
};

export default BanList;
