import React from 'react';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import Alert from '@material-ui/lab/Alert';

// API
import API from 'src/API/API';

export default function BanDialog() {
  const [open, setOpen] = React.useState(false);
  const [alert, setAlert] = React.useState(false);
  const inputElement = React.useRef(null);

  const handleClickOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setAlert(false);
  };

  const addDomainSubmission = async () => {
    if (!inputElement.current.value.includes('.')) {
      setAlert(true);
      return;
    }
    try {
      /*
      * This should be a POST request - not a GET request!
      TODO: Needs to be updated on the server side
      */
      const response = await API.getRequest(`http://localhost/addforbidden?newForbidden=${inputElement.current.value}`);
      if (response.statusCode === '304') {
        console.log(`304 - ${response.responseMessage}`);
      } else if (response.statusCode === '200') {
        console.log(`201 - ${response.responseMessage}`);
      }
    } catch (error) {
      console.log(error.message);
    }
    setOpen(false);
    setAlert(false);
  };

  return (
    <div>
      <Button variant="outlined" color="primary" onClick={handleClickOpen}>
        Add
      </Button>
      <Dialog open={open} onClose={handleClose} aria-labelledby="form-dialog-title">
        <DialogTitle id="form-dialog-title">Subscribe</DialogTitle>
        <DialogContent>
          <DialogContentText>
            To add new domains, simply write the domain name in the field below. The format
            should be &quot;[hostname].[top level domain]&quot;
          </DialogContentText>
          {alert && (<Alert severity="error">The input does not match the format — please try again!</Alert>)}
          <TextField
            autoFocus
            margin="dense"
            id="name"
            label="Domain Name"
            type="email"
            fullWidth
            inputRef={inputElement}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary">
            Cancel
          </Button>
          <Button onClick={addDomainSubmission} color="primary">
            Add
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
