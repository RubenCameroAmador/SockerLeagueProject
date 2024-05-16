# Etapa de ejecución
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Elimina los archivos predeterminados de NGINX
RUN rm -rf ./*

# Copia los archivos publicados desde la etapa de compilación
COPY /bin/publish/wwwroot .

# Exponer el puerto 80
EXPOSE 80

# Inicia NGINX
ENTRYPOINT ["nginx", "-g", "daemon off;"]