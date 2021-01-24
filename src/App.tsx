import { Box, Link } from "@material-ui/core";
import React from "react";
import { Content } from "./components/Content";
import { Header } from "./components/Header";

function App() {
  return (
    <Box
      margin=".75rem"
      marginTop="1.5rem"
      padding=".75rem"
      border={2}
      borderColor="darkblue"
      borderRadius="3rem"
      color="cyan"
      minHeight="80vh"
      marginX="4rem"
    >
      <Header></Header>
      <Content>
        <Link
          style={{ textDecoration: "none", color: "cyan" }}
          href="/Fichas-Tecnicas/App.zip"
        >
          Click Here To Download
        </Link>
      </Content>
      <Content>
        For this application to work you must have 'C:\Data\Fichas_Tecnicas\'
        valid path in your computer
      </Content>
    </Box>
  );
}

export default App;
