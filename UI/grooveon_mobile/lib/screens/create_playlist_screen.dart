import 'dart:io';
import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/providers/image_provider.dart';
import 'package:grooveon_mobile/providers/playlist_provider.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:image_picker/image_picker.dart';
import 'package:grooveon_mobile/helper/image_helper.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';

class CreatePlaylistScreen extends StatefulWidget {
  const CreatePlaylistScreen({super.key});

  @override
  State<CreatePlaylistScreen> createState() =>
      _CreatePlaylistScreenState();
}

class _CreatePlaylistScreenState extends State<CreatePlaylistScreen> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color bg = Color(0xFFF8F6FB);
  static const Color textDark = Color(0xFF1C1C1C);

  final TextEditingController _nameController =
      TextEditingController();
  final TextEditingController _descriptionController =
      TextEditingController();

  bool _isPublic = false;
  File? _selectedImage;

  final PlaylistProvider _playlistProvider = PlaylistProvider();
  bool _isSaving = false;

  @override
  void dispose() {
    _nameController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final image = await ImageHelper.openImagePicker(
      source: ImageSource.gallery,
      imageQuality: 85,
    );

    if (image == null) return;

    setState(() {
      _selectedImage = image;
    });
  }

  Future<void> _savePlaylist() async {
  String? uploadedFileName;

  try {
    final userId = Session.userId;

    if (userId == null) {
      throw Exception("User is not logged in.");
    }

    final name = _nameController.text.trim();
    final description = _descriptionController.text.trim();

    if (name.isEmpty) {
      SnackbarHelper.showError(context, "Playlist name is required.");
      return;
    }

    setState(() {
      _isSaving = true;
    });

    if (_selectedImage != null) {
      uploadedFileName = await ImageAppProvider.upload(
        file: _selectedImage!,
        folder: "playlists",
      );
    }

    await _playlistProvider.insert({
      "userId": userId,
      "name": name,
      "description": description.isEmpty ? null : description,
      "isPublic": _isPublic,
      "coverImageUrl": uploadedFileName,
    });

    if (!mounted) return;

    Navigator.pop(context, true);
  } catch (e) {
    if (uploadedFileName != null) {
      try {
        await ImageAppProvider.delete(
          folder: "playlists",
          fileName: uploadedFileName,
        );
      } catch (_) {}
    }

    if (!mounted) return;

    SnackbarHelper.showError(context, extractErrorMessage(e));
  } finally {
    if (mounted) {
      setState(() {
        _isSaving = false;
      });
    }
  }
}

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: bg,
      appBar: AppBar(
        backgroundColor: bg,
        elevation: 0,
        foregroundColor: textDark,
        title: const Text(
          "Create playlist",
          style: TextStyle(fontWeight: FontWeight.w900),
        ),
      ),
      body: ListView(
        padding: const EdgeInsets.fromLTRB(18, 10, 18, 30),
        children: [
          _imagePickerBox(),
          const SizedBox(height: 22),
          _input(
            controller: _nameController,
            label: "Playlist name",
            hint: "Example: Gym vibes",
          ),
          const SizedBox(height: 14),
          _input(
            controller: _descriptionController,
            label: "Description",
            hint: "Write something about this playlist",
            maxLines: 4,
          ),
          const SizedBox(height: 14),
          _publicSwitch(),
          const SizedBox(height: 24),
          ElevatedButton(
  onPressed: _isSaving ? null : _savePlaylist,
  style: ElevatedButton.styleFrom(
    backgroundColor: primary,
    foregroundColor: Colors.white,
    elevation: 0,
    padding: const EdgeInsets.symmetric(vertical: 15),
    shape: RoundedRectangleBorder(
      borderRadius: BorderRadius.circular(18),
    ),
  ),
  child: _isSaving
      ? const SizedBox(
          width: 20,
          height: 20,
          child: CircularProgressIndicator(
            strokeWidth: 2,
            color: Colors.white,
          ),
        )
      : const Text(
          "Create playlist",
          style: TextStyle(fontWeight: FontWeight.w900),
        ),
),
          const SizedBox(height: 14),
          const Text(
            "Song selection will be added in the next step.",
            textAlign: TextAlign.center,
            style: TextStyle(
              color: Colors.black54,
              fontSize: 13,
            ),
          ),
        ],
      ),
    );
  }

  Widget _imagePickerBox() {
    return InkWell(
      onTap: _pickImage,
      borderRadius: BorderRadius.circular(26),
      child: Container(
        height: 180,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(26),
          border: Border.all(color: const Color(0xFFE6E6EF)),
          image: _selectedImage != null
              ? DecorationImage(
                  image: FileImage(_selectedImage!),
                  fit: BoxFit.cover,
                )
              : null,
        ),
        child: _selectedImage == null
            ? const Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.add_photo_alternate_outlined,
                    color: primary,
                    size: 54,
                  ),
                  SizedBox(height: 10),
                  Text(
                    "Add cover image",
                    style: TextStyle(
                      color: textDark,
                      fontSize: 17,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                  SizedBox(height: 4),
                  Text(
                    "Choose image from gallery.",
                    style: TextStyle(
                      color: Colors.black54,
                      fontSize: 13,
                    ),
                  ),
                ],
              )
            : Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(26),
                  color: Colors.black.withOpacity(0.25),
                ),
                child: const Center(
                  child: Text(
                    "Tap to change cover",
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 15,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                ),
              ),
      ),
    );
  }

  Widget _input({
    required TextEditingController controller,
    required String label,
    required String hint,
    int maxLines = 1,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: const TextStyle(
            color: textDark,
            fontSize: 14,
            fontWeight: FontWeight.w900,
          ),
        ),
        const SizedBox(height: 8),
        TextField(
          controller: controller,
          maxLines: maxLines,
          decoration: InputDecoration(
            hintText: hint,
            filled: true,
            fillColor: Colors.white,
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 16,
              vertical: 14,
            ),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(18),
              borderSide: BorderSide.none,
            ),
          ),
        ),
      ],
    );
  }

  Widget _publicSwitch() {
    return Container(
      padding:
          const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
      ),
      child: SwitchListTile(
        contentPadding: EdgeInsets.zero,
        value: _isPublic,
        activeColor: primary,
        title: const Text(
          "Public playlist",
          style: TextStyle(
            color: textDark,
            fontWeight: FontWeight.w900,
          ),
        ),
        subtitle: const Text(
          "Other users can see this playlist.",
          style: TextStyle(color: Colors.black54),
        ),
        onChanged: (value) {
          setState(() {
            _isPublic = value;
          });
        },
      ),
    );
  }
}