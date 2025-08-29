#ifndef SHADER_H
#define SHADER_H

#include <glad/glad.h>
#include <glm/glm.hpp>
#include <string>
#include <fstream>
#include <sstream>
#include <iostream>

class Shader
{
public:
    unsigned int ID;
    // constructor generates the shader on the fly
    // ------------------------------------------------------------------------
    Shader(const char* commonPath,const char* vertexPath, const char* fragmentPath)
    {
        // Debug dos shaders
        std::cout << "=== Loading Shader ===" << std::endl;
        std::cout << "Common: " << commonPath << std::endl;
        std::cout << "Vertex: " << vertexPath << std::endl;
        std::cout << "Fragment: " << fragmentPath << std::endl;
        
        // 1. retrieve the vertex/fragment source code from filePath
        std::string commonCode;
        std::string vertexCode;
        std::string fragmentCode;
        std::ifstream vShaderFile;
        std::ifstream fShaderFile;
        std::ifstream cShaderFile;
        // ensure ifstream objects can throw exceptions:
        vShaderFile.exceptions (std::ifstream::failbit | std::ifstream::badbit);
        fShaderFile.exceptions (std::ifstream::failbit | std::ifstream::badbit);
    
        try 
        {
            // open files
            vShaderFile.open(vertexPath);
            fShaderFile.open(fragmentPath);
            cShaderFile.open(commonPath);
            std::stringstream vShaderStream, fShaderStream,cShaderStream;
            
            // debug dos shaders
            if (!vShaderFile.is_open()) std::cerr << "ERROR: Vertex shader file missing!" << std::endl;
            if (!fShaderFile.is_open()) std::cerr << "ERROR: Fragment shader file missing!" << std::endl;
            if (!cShaderFile.is_open()) std::cerr << "ERROR: Common shader file missing!" << std::endl;
            
            // read file's buffer contents into streams
            vShaderStream << vShaderFile.rdbuf();
            fShaderStream << fShaderFile.rdbuf();
            cShaderStream << cShaderFile.rdbuf();
            
            // close file handlers
            vShaderFile.close();
            fShaderFile.close();
            cShaderFile.close();
            // convert stream into string
            vertexCode   = vShaderStream.str();
            fragmentCode = cShaderStream.str()+fShaderStream.str();

        }
        catch (std::ifstream::failure& e)
        {
            std::cout << "ERROR::SHADER::FILE_NOT_SUCCESFULLY_READ: " << e.what() << std::endl;
            std::cerr << "Failed file: "
                              << (!vShaderFile.is_open() ? vertexPath :
                                  (!fShaderFile.is_open() ? fragmentPath : commonPath))
                              << std::endl;
        }
        const char* vShaderCode = vertexCode.c_str();
        const char * fShaderCode = fragmentCode.c_str();

        // 2. compile shaders
        unsigned int vertex, fragment;
        // vertex shader
        vertex = glCreateShader(GL_VERTEX_SHADER);
        glShaderSource(vertex, 1, &vShaderCode, NULL);
        glCompileShader(vertex);
        checkCompileErrors(vertex, "VERTEX");
        // fragment Shader
        fragment = glCreateShader(GL_FRAGMENT_SHADER);
        glShaderSource(fragment, 1, &fShaderCode, NULL);
        glCompileShader(fragment);
        checkCompileErrors(fragment, "FRAGMENT");
        // shader Program
        ID = glCreateProgram();
        glAttachShader(ID, vertex);
        glAttachShader(ID, fragment);
        glLinkProgram(ID);
        checkCompileErrors(ID, "PROGRAM");
        // delete the shaders as they're linked into our program now and no longer necessary
        glDeleteShader(vertex);
        glDeleteShader(fragment);
        std::cout << "Shader loaded successfully" << std::endl;
    }
    // activate the shader
    // ------------------------------------------------------------------------
    void use() 
    { 
        glUseProgram(ID); 
    }
    // utility uniform functions
    // ------------------------------------------------------------------------
    void setBool(const std::string &name, bool value) const
    {         
        glUniform1i(glGetUniformLocation(ID, name.c_str()), (int)value); 
    }
    // ------------------------------------------------------------------------
    void setInt(const std::string &name, int value) const
    { 
        glUniform1i(glGetUniformLocation(ID, name.c_str()), value); 
    }
    // ------------------------------------------------------------------------
    void setFloat(const std::string &name, float value) const
    { 
        glUniform1f(glGetUniformLocation(ID, name.c_str()), value); 
    }
    void setSampler(const std::string &name, unsigned int pos) const
    {
         glUniform1i(glGetUniformLocation(ID, name.c_str()), pos);
    }
     void setVec4(const std::string &name, double value[]) const
    {
         glUniform4f(glGetUniformLocation(ID, name.c_str()), value[0], value[1], value[2], value[3]);
    }
     void setVec2(const std::string &name, double value[]) const
    {
         glUniform2f(glGetUniformLocation(ID, name.c_str()), value[0], value[1]);
    }
     void setVec2(const std::string &name, glm::vec2 v) const
    {
         glUniform2f(glGetUniformLocation(ID, name.c_str()), v[0], v[1]);
    }

private:
    // utility function for checking shader compilation/linking errors.
    // ------------------------------------------------------------------------
    void checkCompileErrors(unsigned int shader, std::string type)
    {
        int success;
        char infoLog[1024];
        if (type != "PROGRAM")
        {
            glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
            if (!success)
            {
                glGetShaderInfoLog(shader, 1024, NULL, infoLog);
                std::cout << "ERROR::SHADER_COMPILATION_ERROR of type: " << type << "\n" << infoLog << "\n -- --------------------------------------------------- -- " << std::endl;
            }
        }
        else
        {
            glGetProgramiv(shader, GL_LINK_STATUS, &success);
            if (!success)
            {
                glGetProgramInfoLog(shader, 1024, NULL, infoLog);
                std::cout << "ERROR::PROGRAM_LINKING_ERROR of type: " << type << "\n" << infoLog << "\n -- --------------------------------------------------- -- " << std::endl;
            }
        }
    }
};
#endif

