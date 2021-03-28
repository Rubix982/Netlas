import React from 'react';
import InternalErrorContent from 'src/components/internal/MainContent';

function InternalServerCrash() {
  return (
    <div className={{ margin: '0px', padding: '0px' }}>
      <InternalErrorContent />
    </div>
  );
}
export default InternalServerCrash;
